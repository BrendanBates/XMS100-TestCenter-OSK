using HidSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TestCenter.TestDefinitions;
using TestCenter.TestProcedures;
using TestCenter.TestUI;
//using Wpf.Ui.Controls;
using static TestCenter.App;
using static TestCenter.TestProcedures.Procedure;
using Color = System.Drawing.Color;
using ColorConverter = System.Drawing.ColorConverter;

namespace TestCenter
{
    /// <summary>
    /// Interaction logic for TestsPage.xaml
    /// </summary>
    public partial class TestsPage : Page
    {

        private Procedure procedure = new Procedure();
        private HidDevice hidDevice;
        private const int VendorId = 0x1234;
        private const int ProductId = 0x0001;
        private const int maxTestQuantity = 10;
        private bool OnLoad = false;
        private bool fromSelectBTN = false;
        private SolidColorBrush priorColor;
        private List<byte[]> selectedTestsArray;
        private byte[] results = new byte[64];
        private CancellationTokenSource _readCancellationTokenSource;
        private CancellationTokenSource _testsCancellationTokenSource;
        private List<ToggleButton> testDisplays = new List<ToggleButton>();
        private string[] loadimg = [".", "..", "..."];
        private List<StackPanel> testResults_SP = new List<StackPanel>();
        private List<bool> testResults = new List<bool>();
        private List<byte[]> resetList = new List<byte[]>();
        private uint tempCNT = 0;
        private uint tempCNT2 = 0;
        private bool initSetupCompleted = false;
        private bool FaultOccurrence = false;
        private bool canResponseReceived = false;
        private int phaseTimer = 0;
        private HidStream _currentHidStream;
        private int resetHID_CNT = 0;
        private bool hasPassed = false;
        private bool hasFailed = false;
        int passedTests = 0;
        int failedTests = 0;
        int selectedCount = 0;
        private int faultMessageCount = 0; // Counter for fault CAN messages

        public TestsPage()
        {
            InitializeComponent();
            InitDisplay(App.deviceID);

            if (!OnLoad) {
                RunPageSetup();
                OnLoad = true;
            }
        }

        private void RunPageSetup() {
            SelectAllTests();
        }
        private void InitDisplay(uint _deviceId)
        {
            App.fillAllFuncList(deviceID);
            CreateToggleButtons(TestUIs.testDictionary);
        }

        private void CreateToggleButtons(List<TestDictEntry> testDict)
        {
            int test_CNT = 0;
            foreach (TestDictEntry child in testDict)
            {
                test_CNT++;
                if (child.name != null)
                {
                    // Create a StackPanel to hold the status circle and text
                    var contentPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    // Create the status circle
                    var statusCircle = new Ellipse
                    {
                        Width = 12,
                        Height = 12,
                        Fill = Brushes.Gray, // Default color (not tested yet)
                        Stroke = Brushes.DarkGray,
                        StrokeThickness = 1,
                        Margin = new Thickness(5, 0, 10, 0), // Left margin, right margin for spacing
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    // Create the text label
                    var textLabel = new TextBlock
                    {
                        Text = child.name,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.White // Adjust color as needed
                    };

                    // Add circle and text to the panel
                    contentPanel.Children.Add(statusCircle);
                    contentPanel.Children.Add(textLabel);

                    var newTB = new ToggleButton
                    {
                        Name = "test_" + test_CNT.ToString(),
                        Tag = "toggleTest_" + test_CNT.ToString(),
                        Content = contentPanel, // Use the custom panel instead of just text
                        Style = (Style)FindResource("TestItem"),
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF75AAFF"))
                    };

                    // Store reference to the circle for later updates
                    statusCircle.Name = "StatusCircle_" + test_CNT.ToString();

                    newTB.Click += testSelect;
                    newTB.MouseEnter += MouseOverTestItem;
                    newTB.MouseLeave += MouseExitTestItem;
                    testsPannel.Children.Add(newTB);
                }
            }
        }



        private void UpdateTestStatus(bool _isLastPhase, int testIndex, bool passed, bool finished)
        {
            Dispatcher.Invoke(() =>
            {
                TestDictEntry testEntry = TestUIs.testDictionary[testIndex];
                if (testIndex < testsPannel.Children.Count)
                {
                    var toggleButton = testsPannel.Children[testIndex] as ToggleButton;
                    if (toggleButton?.Content is StackPanel panel)
                    {
                        var circle = panel.Children.OfType<Ellipse>().FirstOrDefault();
                        if (circle != null)
                        {
                            if (passed && _isLastPhase && !hasFailed)
                            {
                                circle.Fill = Brushes.LightGreen;
                                circle.Stroke = Brushes.MediumSpringGreen;
                                testEntry.result = true;
                            }
                            else if (finished && _isLastPhase)
                            {
                                circle.Fill = Brushes.Red;
                                circle.Stroke =Brushes.DarkRed;
                                testEntry.result = false;
                            }
                            else
                            {
                                // In-progress test - show white/gray
                                circle.Fill = Brushes.White;
                                circle.Stroke = Brushes.LightGray;
                                testEntry.result = false; // Default to false until test completes
                            }
                            //circle.Fill = (passed && _isLastPhase && !hasFailed) ? Brushes.Green :
                            //                    (finished && _isLastPhase) ? Brushes.Red : Brushes.White;
                            //circle.Stroke =  (passed && _isLastPhase && !hasFailed) ? Brushes.DarkGreen :
                            //                    (finished && _isLastPhase) ? Brushes.DarkRed : Brushes.LightGray;
                            TestUIs.testDictionary[testIndex] = testEntry;
                        }
                    }
                }
                // Update progress bar when test phase completes
                UpdateProgressBar();            
            });
        }

        private void UpdateProgressBar()
        {
            Dispatcher.Invoke(() =>
            {
                // Count completed tests
                int completedTests = 0;
                int totalTests = TestUIs.testDictionary.Count;
                
                foreach (var testEntry in TestUIs.testDictionary)
                {
                    // Check if test has been completed (either passed or failed)
                    var toggleButton = testsPannel.Children.OfType<ToggleButton>()
                        .ElementAtOrDefault(TestUIs.testDictionary.IndexOf(testEntry));
                    
                    if (toggleButton?.Content is StackPanel panel)
                    {
                        var circle = panel.Children.OfType<Ellipse>().FirstOrDefault();
                        if (circle != null && (circle.Fill == Brushes.LightGreen || circle.Fill == Brushes.Red))
                        {
                            completedTests++;
                        }
                    }
                }
                
                // Calculate progress percentage
                double progressPercentage = selectedCount > 0 ? (double)completedTests / selectedCount * 100 : 0;
                testProgressBar.Value = progressPercentage;
                
                // Update progress text
                if (completedTests == 0)
                {
                    progressText.Text = "Starting Tests...";
                    statusText.Text = $"Test 1/{selectedCount} in progress...";
                }
                else if (completedTests == selectedCount)
                {
                    passedTests = TestUIs.testDictionary.Count(t => t.result);
                    failedTests = selectedCount - passedTests;
                    progressText.Text = $"Complete: {passedTests} Passed, {failedTests} Failed";
                    passedCNTText.Text = $"{passedTests}/{selectedCount}";
                    passedCNTText.Foreground = Brushes.LightGreen;
                    failedCNTText.Foreground = Brushes.Red;
                    failedCNTText.Text = $"{failedTests}/{selectedCount}";
                    resultsText.Text = (failedTests > 0) ? "FAIL" : "PASS";
                    resultsText.Foreground = (failedTests > 0) ? Brushes.Red : Brushes.LightGreen;

                    // Change progress bar color based on results
                    if (failedTests == 0)
                    {
                        testProgressBar.Foreground = Brushes.LightGreen;
                    }
                    else
                    {
                        testProgressBar.Foreground = Brushes.Red;
                    }
                }
                else
                {
                    passedTests = TestUIs.testDictionary.Count(t => t.result);
                    failedTests = completedTests - passedTests;
                    progressText.Text = $"{completedTests}/{selectedCount} Tests Complete";
                    statusText.Text = (selectedCount > completedTests) ? $"Test {completedTests+1}/{selectedCount} in progress..." : "Testing Complete";
                    passedCNTText.Foreground = (passedTests>0)?Brushes.LightGreen:Brushes.White;
                    failedCNTText.Foreground = (failedTests>0)?Brushes.Red:Brushes.White;
                    passedCNTText.Text = $"{passedTests}/{selectedCount}";
                    failedCNTText.Text = $"{failedTests}/{selectedCount}";
                }
            });
        }


        private void ResetAllTestStatus()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (ToggleButton toggleButton in testsPannel.Children.OfType<ToggleButton>())
                {
                    if (toggleButton.Content is StackPanel panel)
                    {
                        var circle = panel.Children.OfType<Ellipse>().FirstOrDefault();
                        if (circle != null)
                        {
                            circle.Fill = Brushes.Gray;
                            circle.Stroke = Brushes.DarkGray;
                        }
                    }
                }
            });
        }


        private void CreateTestDescriptions(List<TestDictEntry> testDict)
        {
            Dispatcher.Invoke(() =>
            {
                rslts_SP.Children.Clear();
                testResults_SP.Clear();

                int test_CNT = 0;
                foreach (TestDictEntry child in testDict)
                {
                    if (child.name != null)
                    {
                        var newResultName = new TextBlock
                        {
                            Text = procedure.selectedTestEntries[test_CNT].name,
                            Margin = new Thickness(2),
                            FontWeight = FontWeights.Bold,
                            Style = (Style)FindResource("TestText"),
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            TextDecorations = TextDecorations.Underline,
                            Padding = new Thickness(5, 2, 0, 0),
                            Height = 20
                        };
                        //newResultName.MouseEnter += MouseOverResultItem;
                        //newResultName.MouseLeave += MouseExitResultItem;
                        rslts_SP.Children.Add(newResultName);

                        var newResultSP = new StackPanel
                        {
                            Name = "result_" + test_CNT.ToString()
                        };
                        testResults_SP.Add(newResultSP);
                        rslts_SP.Children.Add(newResultSP);

                        var newDescHeader = new TextBlock
                        {
                            Text = "Description: ",
                            Margin = new Thickness(2),
                            FontWeight = FontWeights.Bold,
                            Style = (Style)FindResource("TestText"),
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Padding = new Thickness(25, 1, 0, 0),
                            Height = 20
                        };
                        testResults_SP[test_CNT].Children.Add(newDescHeader);

                        var newDesc = new TextBlock
                        {
                            Text = child.description,
                            Margin = new Thickness(0),
                            FontWeight = FontWeights.Bold,
                            Style = (Style)FindResource("TestText"),
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Padding = new Thickness(60, 0, 0, 0),
                            TextWrapping = TextWrapping.Wrap
                        };
                        testResults_SP[test_CNT].Children.Add(newDesc);

                        test_CNT++;
                    }
                }
            });
        }

        public void updateResultsDisplay()
        {

            Debug.WriteLine("START UPDATING RESULTS");
            int test_CNT = 0;
            foreach (TestDictEntry child in procedure.selectedTestEntries)
            {

                if (child.testID != null)
                {
                    // Keep only the description header (index 0) and description text (index 1)
                    // Clear the rest
                    while (testResults_SP[test_CNT].Children.Count > 2)
                    {
                        testResults_SP[test_CNT].Children.RemoveAt(2);
                    }

                    int phaseCNT = 0;

                    // Setup the phase results section for the current test being processed
                    var newPhaseSection = new TextBlock
                    {
                        Text = $"Phases: ",
                        Margin = new Thickness(0),
                        FontWeight = FontWeights.Bold,
                        Style = (Style)FindResource("TestText"),
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Padding = new Thickness(25, 0, 0, 0),
                        Height = 30
                    };
                    // Outside of following foreach loop so the section title isn't displayed for each phase of the test
                    // which would result in a messy display
                    testResults_SP[test_CNT].Children.Add(newPhaseSection); // Add the phase section just created to the testResults stack pannel

                    foreach (TestPhase phase in procedure.selectedFunc_List)
                    {
                        if(phase.parentID == child.testID)
                        {
                            // Create the text block that will display the current phases name
                            var newPhaseTitle = new TextBlock
                            {
                                Text = $"{phase.name} ",
                                Margin = new Thickness(0),
                                FontWeight = FontWeights.Bold,
                                Style = (Style)FindResource("TestText"),
                                Foreground = Brushes.White,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Padding = new Thickness(40, 0, 0, 0),
                                Height = 30
                            };
                            // Create the text block for displaying the result of the phase
                            var newPhaseResult = new TextBlock
                            {
                                Text = $"Results: {phase.result}",
                                Margin = new Thickness(0),
                                FontWeight = FontWeights.Bold,
                                Style = (Style)FindResource("TestText"),
                                Foreground = Brushes.White,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Padding = new Thickness(60, 0, 0, 0),
                                Height = 30
                            };
                            // The following adds the phase name and result to the stack pannel
                            testResults_SP[test_CNT].Children.Add(newPhaseTitle);
                            testResults_SP[test_CNT].Children.Add(newPhaseResult);
                            phaseCNT++;
                        }
                    }

                    var newResult = new TextBlock
                    {
                        Text = $"{child.name} Result: " + (TestUIs.testDictionary[test_CNT].result ? "Pass" : "Fail"),
                        Margin = new Thickness(0),
                        FontWeight = FontWeights.Bold,
                        Style = (Style)FindResource("TestText"),
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Padding = new Thickness(25, 0, 0, 0),
                        Height = 30
                    };

                    var newDetailsHeader = new TextBlock
                    {
                        Text = "Details:",
                        Margin = new Thickness(0),
                        FontWeight = FontWeights.Bold,
                        Style = (Style)FindResource("TestText"),
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Padding = new Thickness(25, 0, 0, 0),
                        Height = 20
                    };

                    var newDetails = new TextBlock
                    {
                        Text = child.causes, //procedure.selectedTestResponse[child.testID].description,
                        Margin = new Thickness(0),
                        FontWeight = FontWeights.Bold,
                        Style = (Style)FindResource("TestText"),
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Padding = new Thickness(40, 0, 0, 0),
                        Height = 30
                    };

                    testResults_SP[test_CNT].Children.Add(newResult);
                    testResults_SP[test_CNT].Children.Add(newDetailsHeader);
                    testResults_SP[test_CNT].Children.Add(newDetails);

                    test_CNT++;
                }
            }
            Debug.WriteLine("DONE UPDATING RESULTS");
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            Switcher.Switch(new HomePage());
        }

        private void ResultsToggle_Click(object sender, RoutedEventArgs e)
        {
            // Ensure only one toggle is checked
            ResultsToggle.IsChecked = true;
            CANDataToggle.IsChecked = false;

            // Show Results content, hide CAN Data content
            ResultsScrollViewer.Visibility = Visibility.Visible;
            CANDataScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void CANDataToggle_Click(object sender, RoutedEventArgs e)
        {
            // Ensure only one toggle is checked
            CANDataToggle.IsChecked = true;
            ResultsToggle.IsChecked = false;

            // Show CAN Data content, hide Results content
            CANDataScrollViewer.Visibility = Visibility.Visible;
            ResultsScrollViewer.Visibility = Visibility.Collapsed;
        }


        private void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                testsPannel.Children.OfType<ToggleButton>().ToList().ForEach(t => t.IsEnabled = false); // Disable test selection during run
                RunBTN.Content = "Running...";
                SelectAllBTN.IsEnabled = false;
                // Clean up previous run
                CleanupPreviousRun();

                // Reset state variables
                ResetStateVariables();

                // Reset test status indicators
                ResetAllTestStatus();

                // Check if the HID device is initialized, if not, initialize it
                if (hidDevice == null)
                {
                    InitializeHidDevice();
                }
                // Fill the CMD Packet array with data for tests and send
                // Simple test - send some data
                //intprocedure.buildHeader(definitions.tests);
                if (hidDevice != null)
                {
                    // Dispose previous stream if it exists
                    _currentHidStream?.Dispose();
                    //initSetupCompleted = false;
                    App.fillAllFuncList(deviceID);
                    resetList = procedure.getResetList(IsResetListSet);
                    procedure.fillSelected();
                    selectedTestsArray = procedure.runSelected();
                    _testsCancellationTokenSource = new CancellationTokenSource();

                    selectedCount = testsPannel.Children.OfType<ToggleButton>()
                        .Count(tb => tb.IsChecked == true);
                    NumTestsDisplay.Text = $"Selected Tests: {selectedCount}/{TestUIs.testDictionary.Count}";

                    Debug.WriteLine($"=== SELECTION DEBUG ===");
                    Debug.WriteLine($"Selected toggle buttons: {selectedCount}");
                    Debug.WriteLine($"selectedTestsArray.Count: {selectedTestsArray.Count}");
                    Debug.WriteLine($"procedure.selectedFunc_List.Count: {procedure.selectedFunc_List.Count}");
                    Debug.WriteLine($"TestUIs.testDictionary.Count: {TestUIs.testDictionary.Count}");
                    Debug.WriteLine($"======================");

                    CreateTestDescriptions(procedure.selectedTestEntries);
                    // Open HIDStream 
                    //HidStream hidStream = null;
                    if (hidDevice.TryOpen(out _currentHidStream))
                    {
                        // Pass the stream to your test runner
                        _ = runCMDs(_testsCancellationTokenSource.Token, _currentHidStream);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during test execution: {ex.Message}");
                RunBTN.Content = "Error";
            }
            finally
            {
                RunBTN.Content = "Run";
            }
        }



        private void CleanupPreviousRun()
        {
            // Cancel any existing tasks
            _testsCancellationTokenSource?.Cancel();
            _readCancellationTokenSource?.Cancel();

            // Wait a bit for tasks to complete
            Task.Delay(100).Wait();

            // Dispose old cancellation tokens
            _testsCancellationTokenSource?.Dispose();
            _readCancellationTokenSource?.Dispose();

            _testsCancellationTokenSource = null;
            _readCancellationTokenSource = null;

            //// Reset test dictionary results
            //for (int ii = 0; ii < TestUIs.testDictionary.Count; ii++)
            //{
            //    TestDictEntry entry = TestUIs.testDictionary[ii];
            //    entry.result = false; // Set to fail by default
            //    entry.currentPhase = 0;
            //    TestUIs.testDictionary[ii] = entry;
            //}

            // Note: procedure.selectedFunc_List will be cleared and rebuilt by fillSelected()
            // so we don't need to reset it here
        }

        private void ResetStateVariables()
        {
            FaultOccurrence = false;
            phaseTimer = 0;  // Unified timer for both fault and AI tests
            initSetupCompleted = false;
            canResponseReceived = false;
            tempCNT = 0;
            tempCNT2 = 0;
            hasPassed = false;      
            hasFailed = false;        
            passedTests = 0;        
            failedTests = 0;        
            resetHID_CNT = 0;


            // Clear UI panels
            Dispatcher.Invoke(() =>
            {
                error_SP.Children.Clear();
                warning_SP.Children.Clear();
                status_SP.Children.Clear();
                rslts_SP.Children.Clear();
                testResults_SP.Clear();
            });
        }




        private async Task runCMDs(CancellationToken cancellationToken, HidStream _hidStream)
        {
            int phaseIDX = 0;
            int phaseOfTest = 0;
            int lastIDX = 1;
            int currentTestID = -1;
            int totalPhases = selectedTestsArray.Count;
            bool cancelTask = cancellationToken.IsCancellationRequested;
            bool isLastPhase = false;
            TestPhase currentPhase = procedure.selectedFunc_List[phaseIDX];
            byte faultID = currentPhase.associatedFault; // Sets the current faultID to the associated fault identifier 
            int fltIDX = faultID & 0x0F; // Sets the fault index to be tested to the expected faults index
            int fltListNum = (faultID >> 4) - 1; // Sets the faultList to be tested based on the expected faults faultList location
            FaultResponse associatedFault;
            if (currentPhase.testType == CHECK_FAULT)
            {
                associatedFault = MasterFaultList[fltListNum][fltIDX];
            }
            //TestDictEntry entry = TestUIs.testDictionary[currentPhase.parentID];
            HidStream currentStream = _hidStream;

            try
            {
                // Pass hidStream to SendHidData and getResults
                #if DEBUG
                    Debug.WriteLine($"=== STARTING TESTS RUN ===");
                #endif
                await clearFaults(currentPhase.testType, currentStream, cancellationToken); // Clear board under tests initial faults and start simulating working system
                                                                     //await SendHidData(selectedTestsArray[0], currentStream);
                hasFailed = false;
                hasPassed = false;

                while (!cancelTask && (phaseIDX < totalPhases))
                {

                    if (phaseIDX != lastIDX)
                    {
                        currentPhase = procedure.selectedFunc_List[phaseIDX]; // Update the current phase being tested 
                        await clearFaults(currentPhase.testType, currentStream, cancellationToken);

                        if (currentPhase.parentID != currentTestID) // Reset for next test
                        {
                            hasFailed = false;
                            hasPassed = false;
                            phaseOfTest = 0;
                            currentTestID = currentPhase.parentID;
                            UpdateTestStatus(isLastPhase, currentTestID, false, false);
                        }
                        if (currentPhase.testType == CHECK_FAULT)
                        { 
                            faultID = currentPhase.associatedFault; // Sets the current faultID to the associated fault identifier 
                            fltIDX = faultID & 0x0F; // Sets the fault index to be tested to the expected faults index
                            fltListNum = (faultID >> 4) - 1; // Sets the faultList to be tested based on the expected faults faultList location

                            //await ResetAllFaultStatuses(currentStream, cancellationToken);
                            associatedFault = MasterFaultList[fltListNum][fltIDX];
                        }
#if DEBUG
                        Debug.WriteLine($"=== TEST #{currentPhase.parentID} PHASE #{phaseOfTest++} STARTING ===");
                        //Debug.WriteLine($"  SENDING TEST DATA");
#endif
                        await SendHidData(selectedTestsArray[phaseIDX], currentStream, cancellationToken);
                        phaseTimer = 0; // Reset unified timer for next test phase
#if DEBUG
                        //Debug.WriteLine($"  TEST DATA SENT");
#endif
                        lastIDX = phaseIDX;
                    }

                    if (totalPhases != phaseIDX + 1)
                    {
                        TestPhase nextPhase = procedure.selectedFunc_List[phaseIDX + 1]; // Update the next phase to be tested 
                        isLastPhase = (nextPhase.parentID != currentPhase.parentID);
                    }
                    else
                    {
                        isLastPhase = true;
                    }
                    // **OPTIMIZED TEST EXECUTION LOGIC**
                    // Each test phase now has dedicated, non-interfering execution paths

                    if (currentPhase.testType == CHECK_FAULT)
                    {
                        // FAULT-BASED TEST: Monitor for fault occurrence/absence
                        phaseIDX = await ExecuteFaultBasedTest(phaseIDX, lastIDX, currentTestID, isLastPhase, cancellationToken);
                    }
                    else if (currentPhase.testType == CHECK_AI)
                    {
                        // ANALOG INPUT TEST: Monitor for AI value changes
                        phaseIDX = await ExecuteAnalogInputTest(phaseIDX, isLastPhase, cancellationToken);
                    }
                    else
                    {
                        Debug.WriteLine($"WARNING: Unknown test type {currentPhase.testType} for phase {phaseIDX}");
                        phaseIDX++; // Skip unknown test types
                    }
                    phaseTimer++; // Increment unified timer for each loop iteration
                    
                    await Task.Delay(5, cancellationToken);
                }
                //Debug.WriteLine($"=== TESTS COMPLETED ===");
                Dispatcher.Invoke(() => { RunBTN.Content = "Finished"; });
            }
            catch (Exception ex)
            {
                //stopReading();
                Debug.WriteLine($"EXCEPTION IN runCMDs: {ex.Message} at test index = {phaseIDX}");
            }
            finally
            {
                Debug.WriteLine($"=== CLEANUP STARTED ===");
                await clearFaults(currentPhase.testType, currentStream, cancellationToken);
                // STOP getResults FIRST before disposing the stream
                stopReading(); // This cancels _readCancellationTokenSource

                // Give it time to stop
                await Task.Delay(100);
                currentStream?.Dispose(); 
                // Re-enable test selection buttons
                testsPannel.Children.OfType<ToggleButton>().ToList().ForEach(t => t.IsEnabled = true); // Re-enable toggle buttons after run
                SelectAllBTN.IsEnabled = true; // Re-enable Select All button after run
                // UI updates must be on the UI thread
                Dispatcher.Invoke(() =>
                {
                    updateResultsDisplay();
                    RunBTN.Content = "Run";
                });
                Debug.WriteLine($"=== CLEANUP COMPLETED ===");
            }
        }




        private async Task clearFaults (byte testType, HidStream _hidStream, CancellationToken cancellationToken)
        {
#if DEBUG
            Debug.WriteLine("   _-_ STARTING CLEAR FAULT _-_");
#endif
            await runResetCommands(_hidStream, cancellationToken);
            if (testType == CHECK_FAULT)
            {
                await ResetAllFaultStatuses(_hidStream, cancellationToken);
                FaultOccurrence = false;
            }
            // Delay to ensure interface board processes reset
            //await Task.Delay(100, cancellationToken); 
        }
        private async Task ResetAllFaultStatuses(HidStream _hidStream, CancellationToken cancellationToken)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                //Debug.WriteLine($"      CLEARING FAULT LIST #{ii+1}");
                for (int jj = 0; jj < MasterFaultList[ii].Length; jj++)
                    MasterFaultList[ii][jj].status = false;
            }
            //        Debug.WriteLine($"Fault ID: {MasterFaultList[ii][jj].faultID} Status = {MasterFaultList[ii][jj].status}");
        }

        private async Task runResetCommands(HidStream _hidStream, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"       resetlist count = {resetList.Count}");
            for (int ii = 0; ii < resetList.Count; ii++)
            {
                //RunBTN.Content = "In Progress" + loadimg[phaseIDX % 3];
                Debug.WriteLine($"       running resetList[{ii}]...");
                await SendHidData(resetList[ii], _hidStream, cancellationToken);
                //await Task.Delay(200, cancellationToken);
            }
            if (initSetupCompleted == false)
            {
                    stopReading();
                    _readCancellationTokenSource = new CancellationTokenSource();
                    _ = getResults(_readCancellationTokenSource.Token, _hidStream);
                    initSetupCompleted = true;
            }
        }


        private async Task getResults(CancellationToken cancellationToken, HidStream stream)
        {
            Debug.WriteLine("=== getResults STARTED ===");
            try
            {
                byte[] response = new byte[65];
                int readCount = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (stream != null && stream.CanRead)
                    {

                        try
                        {
                            // Read in a background thread to avoid blocking U

                            //stream.ReadTimeout = 100;
                            await Task.Run(() => { stream.Read(response); });

                            checkResponse(response);
                        }
                        catch (TimeoutException)
                        {
                            Debug.WriteLine("getResults: Timeout occurred");
                            // Optionally wait before retrying
                            await Task.Delay(100, cancellationToken);
                            //stopReading();
                            //MessageBox.Show("ERROR: Reading Has Been Cancelled Due to a Time Out Exception.");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"getResults: Read error: {ex.Message}");
                            await Task.Delay(100, cancellationToken);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("getResults: Stream not available");
                        await Task.Delay(100, cancellationToken);
                    }
                    }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("getResults: Reading cancelled.");
            }
            catch (Exception ex) {
                //stopReading();
                Debug.WriteLine($"getResults: Error: {ex.Message}");
            }
            Debug.WriteLine("=== getResults ENDED ===");
        }


        private void checkResponse(byte[] response)
        {
            int buff = 3; // Account for the bytes used to identify the response 1 and 2
                          // index 0 is used by the HID service routine
            uint offsetSecondHalf = 31;
            uint isSecondHalf = response[2];
            int maxAnIDX = (response[2] == 0) ? 31 : 19;
            int n = 0;
            //Debug.WriteLine("   checkResponse Started");
            if (response[1] == 'A') // Relates to returnResults() function in Main.c of XMS101 and XMS102
            {                       // The embedded software for the board tester
                if ((response[2] == 0) || (response[2] == 1))
                {
                    for (int ii = 0; ii < maxAnIDX; ii++)
                    {
                        int _base = (2 * ii) + buff;
                        AI_Vals[ii + (offsetSecondHalf * isSecondHalf)] = response[_base] | response[_base + 1] << 8;
                    }
                    //checkAIs();
#if DEBUG
                    //Debug.WriteLine($" HOIST UP: {AI_Vals[AI_01]} HOIST DOWN: {AI_Vals[AI_02]} REM1 NO: {AI_Vals[AI_13]} REM1 NC: {AI_Vals[AI_14]} AUX: {AI_Vals[AI_07]} HTSNK: {AI_Vals[AI_04]}");
                    //Debug.Write("ANALOG:  ");
                    //while (n < AI_Vals.Length)
                    //{
                    //    Debug.Write($"{n}={AI_Vals[n++]}  ");
                    //}
                    //Debug.Write("\n");

                    /*IDX_HOIST_HU1 = AI_01; 
                     *IDX_HOIST_HD1 = AI_02;  
                     *IDX_SHUNT = AI_03;   
                     *IDX_FANS_HEAT = AI_04;  
                     *IDX_FANS_XFMR = AI_05;  
                     *IDX_28V_CONT = AI_06;   
                     *IDX_OUT_CONT = AI_07;   
                     *IDX_ESIG = AI_08;       
                     *IDX_REM_FLT_NO = AI_09; 
                     *IDX_REM_FLT_NC = AI_10; 
                     *IDX_DOOR_INTLK = AI_11; 
                     *IDX_ESTOP = AI_12;  //  
                     *IDX_REM1_NO = AI_13;  / 
                     *IDX_REM1_NC = AI_14;  / 
                     *IDX_REM2_NO = AI_15;  / 
                     *IDX_REM2_NC = AI_16;  / 
                     *IDX_REM_PWR_NO = AI_17; 
                     *IDX_REM_PWR_NC = AI_18; 
                     * 
                     * 
                     */
#endif
                }
                else if (response[2] == 'K')
                {
                        Debug.WriteLine($"Command Acknowledgement Recieved CMD TYPE: {response[14].ToString("X2")}");
                }
                else
                {
                    Debug.WriteLine("ACK/INPUT ERROR: Invalid response." + (char)response[3] + (char)response[4] + "\n");
                    return; // invalid response
                }
                    
            }
            else if (response[1] == 'C' && response[2] == 'R') {
                processCAN_RX(response);
                tempCNT++;
                //Debug.WriteLine(string.Join(" , ", response));
            }
            else if (response[1] == 'E' && response[2] == 'R')
            {
                // Handle error response
#if DEBUG
                Debug.WriteLine("ERROR " + (char)response[1] + (char)response[2] + ":" + (char)response[3] + (char)response[4] + " " + string.Join(" , ", response));
                Debug.WriteLine(getError(response));
#else
                                                Console.WriteLine("ERROR " + (char)response[1] + (char)response[2] + ":" + (char)response[3] + (char)response[4] + " " + string.Join(" , ", response));
                                                Console.WriteLine(getError(response));
#endif
                //MessageBox.Show("Test Failed: \nTest received by PCB, but returned error. \n" +
                //"\nERROR: " + getError(response));
            }
            else
            {
                //Debug.WriteLine("Waiting on acknowledgement response...");
                //Debug.WriteLine(string.Join(" , ", response));
            }
            for (int ii = 0; ii < 64; ii++)
                results[ii] = response[ii];
            //Debug.WriteLine("   checkResponse Ended");
        }



        public void processCAN_RX(byte[] response)
        {
            
            // The reason for response[7] not being shifted 8 is that the data bytes are organized little-endian in byte sized parts
            // EX: [ID], [7,...,0], [15,...,8] and repeated for the rest of the packet.
            uint canResp_ID = (uint)(response[3] << 24) | (uint)(response[4] << 16) | (uint)(response[5] << 8) | (uint)response[6]; // [ID]
            uint data1 = (uint)(response[8] << 8) | response[7];
            uint data2 = (uint)(response[10] << 8) | response[9];
            uint data3 = (uint)(response[12] << 8) | response[11];

            //Debug.WriteLine("       Process CAN RX Started");

            Dispatcher.Invoke(() =>
            {
                // Get the appropriate StackPanel based on status
                StackPanel targetPanel = status_SP;
                Brush textColor = Brushes.White;

                if (canResp_ID == StatusID_1 || canResp_ID == StatusID_2)
                {
                    string canResp_Data = $"Data1: {(uint)(response[8] << 8) | response[7]}   " + // response[7] = [7,...,0] and response[8] = [15,...,8]
                            $"Data2: {(uint)(response[10] << 8) | response[9]}   " +
                            $"Data3: {(uint)(response[12] << 8) | response[11]}   " +
                            $"Data4: {(uint)/*(response[13] << 8) | */
                    response[14]}";
                    switch (response[14])
                    {
                        case 0xFF: // Fault
                            uint statusData = 0;
                            faultMessageCount++;
                            //Debug.WriteLine(canResp_ID + ": " + canResp_Data);
                            for (int ii = 0; ii < MasterFaultList.Count; ii++)
                            {
                                statusData = (ii == 0) ? data1 : (ii == 1) ? data2 : data3;
                                for (int jj = 0; jj < MasterFaultList[ii].Length; jj++)
                                {
                                    int bitNum = MasterFaultList[ii][jj].bitNUM;
                                    MasterFaultList[ii][jj].status = ((statusData & (1 << bitNum)) != 0);
                                    //Debug.WriteLine($"{MasterFaultList[ii][jj].faultID} = {MasterFaultList[ii][jj].status}");
                                }
                            }
                            updateStackPanels(error_SP, Brushes.Red, canResp_ID, canResp_Data, $"FAULT ID (Count: {faultMessageCount})");
                            FaultOccurrence = true;
                            break;

                        case 0xFE: // Warning
                            updateStackPanels(warning_SP, Brushes.Yellow, canResp_ID, canResp_Data, "WARNING ID");
                            break;

                        default: // Status
                            updateStackPanels(status_SP, Brushes.White, canResp_ID, canResp_Data, "STATUS ID");
                            break;
                    }
                }
                else if (canResp_ID == AnalogAI_ID1 || canResp_ID == AnalogAI_ID2)
                {
                    string canResp_Data = 
                            $"AIN A: {(uint)(response[8] << 8) | response[7]}  " + // response[7] = [7,...,0] and response[8] = [15,...,8]
                            $"AIN B: {(uint)(response[10] << 8) | response[9]}   " +
                            $"AIN C: {(uint)(response[12] << 8) | response[11]}   ";
                    updateStackPanels(analogs_SP, Brushes.White, canResp_ID, canResp_Data, "INPUT AMPS");
                }
                else if (canResp_ID == AnalogVI_ID1 || canResp_ID == AnalogVI_ID2)
                {
                    string canResp_Data =
                            $"VIN A: {(uint)(response[8] << 8) | response[7]}   " + // response[7] = [7,...,0] and response[8] = [15,...,8]
                            $"VIN B: {(uint)(response[10] << 8) | response[9]}   " +
                            $"VIN C: {(uint)(response[12] << 8) | response[11]}   ";
                    updateStackPanels(analogs_SP, Brushes.White, canResp_ID, canResp_Data, "INPUT VOLTS");
                }
                else if (canResp_ID == AnalogAO_ID1 || canResp_ID == AnalogAO_ID2)
                {
                    string canResp_Data =
                            $"AOUT A: {(uint)(response[8] << 8) | response[7]}   " + // response[7] = [7,...,0] and response[8] = [15,...,8]
                            $"AOUT B: {(uint)(response[10] << 8) | response[9]}   " +
                            $"AOUT C: {(uint)(response[12] << 8) | response[11]}   " +
                            $"Hz Value: {(uint)(response[14] << 8) | response[13]}";
                    updateStackPanels(analogs_SP, Brushes.White, canResp_ID, canResp_Data, "OUTPUT AMPS");
                }
                else if (canResp_ID == AnalogVO_ID1 || canResp_ID == AnalogVO_ID2)
                {
                    string canResp_Data =
                            $"VOUT A: {(uint)(response[10] << 8) | response[9]}   " + // response[7] = [7,...,0] and response[8] = [15,...,8]
                            $"VOUT B: {(uint)(response[12] << 8) | response[11]}   " +
                            $"VOUT C: {(uint)(response[14] << 8) | response[13]}   ";
                    updateStackPanels(analogs_SP, Brushes.White, canResp_ID, canResp_Data, "OUTPUT VOLTS");
                }
                else if (canResp_ID == Analog28V_ID1 || canResp_ID == Analog28V_ID2)
                {
                    string canResp_Data =
                            $"28V VOUT: {(uint)(response[8] << 8) | response[7]}  ";
                    updateStackPanels(analogs_SP, Brushes.White, canResp_ID, canResp_Data, "OUTPUT VOLTS");
                }

            });
            //Debug.WriteLine("       Process CAN RX Ended");
        }

        private void updateStackPanels(StackPanel sp, Brush _textColor, uint _ID, string dataString, string prefix = "CAN ID")
        {
            bool found = false;

            // Try to find existing TextBlock in the target panel
            foreach (var child in sp.Children)
            {
                if (child is System.Windows.Controls.TextBlock tb && tb.Tag is uint tagID && tagID == _ID)
                {
                    // Update existing TextBlock
                    tb.Text = $"{prefix}: {_ID}\n       {dataString}";
                    tb.Foreground = _textColor;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // Create new TextBlock
                var newTB = new System.Windows.Controls.TextBlock
                {
                    Tag = _ID,
                    Text = $"{prefix}: {_ID}\n       {dataString}",
                    Margin = new Thickness(2),
                    FontWeight = FontWeights.Bold,
                    Style = (Style)FindResource("TestText"),
                    Foreground = _textColor,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(5),
                    Height = 55
                };
                sp.Children.Add(newTB);
            }
        }

        private void stopReading()
        {
            if (_readCancellationTokenSource != null)
            {
                _readCancellationTokenSource.Cancel();
                _readCancellationTokenSource.Dispose();
                _readCancellationTokenSource = null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitializeHidDevice()
        {
            try
            {
                var deviceList = DeviceList.Local;
                var hidDeviceList = deviceList.GetHidDevices().ToList();

                // Find your specific device by VID/PID
                hidDevice = hidDeviceList.FirstOrDefault(d =>
                    d.VendorID == VendorId &&
                    d.ProductID == ProductId);

                if (hidDevice == null)
                {
                    MessageBox.Show("Device not found! Check if USB is connected.");
                    return;
                }

                // Open the device
                if (!hidDevice.TryOpen(out HidStream testStream))
                {
                    MessageBox.Show("Failed to open device!");
                    return;
                }
                testStream.Dispose();
                Debug.WriteLine("HID device opened successfully!");

                string mfc = hidDevice.GetManufacturer();
                string prod = hidDevice.GetProductName();
                string snum = "12345";
                // The following throws an exception
                // Simple test - read the device info 
                Debug.WriteLine($"Connected to:\nManufacturer: " + mfc + "\n" +
                               $"Product: " + prod + "\n" +
                               $"Serial Number: " + snum);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing HID device: {ex.Message}");
            }
        }
        


        private string getError(byte[] response)
        {
            if (response.Length < 8 || response[1] != 'E' || response[2] != 'R')
                return "Invalid error response format";
            // Check the error response type and return associated message
            return (response[3], response[4]) switch {
                ((byte)'N', (byte)'C') => "No Command - numCMDs = 0. Check: definitions.tests_List and testPKTs occurrences.",
                ((byte)'I', (byte)'C') => "Invalid Command\n - cmdType unrecognized\n" + "Test ID = " + response[5] + "\nCommand Type = " + response[6],
                ((byte)'I', (byte)'P') => "Invalid Pin \n" + "Test ID = " + response[5] + "\nPin Number = " + response[7],
                ((byte)'C', (byte)'T') => "CAN Transmission",
                ((byte)'C', (byte)'R') => "CAN Reception", 
                ((byte)'I', (byte)'E') => "Invalid Expander\n" + "Test ID = " + response[5] + "\nCommand Type = Digital" + "\nExpander = " + response[6]%0x10,
                ((byte)'E', (byte)'P') => "Invalid Expander Port\n" + "Test ID = " + response[5] + "\nCommand Type = Digital" + "\nExpander = " + response[6] % 0x10 + "\nPort = " + response[7]%8,
                ((byte)'C', (byte)'B') => "CAN Buffer Error\n" + "Test ID = " + response[5],
                _ => "Unknown Error" // Default case for unexpected values
            };
        }

        private async Task SendHidData(byte[] data, HidStream stream, CancellationToken cancellationToken)
        {
            try
            {
                //stopReading();
                stream.Write(data);
                // Read response if expected
                //byte[] response = new byte[64]; // Adjust buffer size as needed
//                while (response[1] != 'A' && response[2] != 'K')
//                {
//                    int count = stream.Read(response);
//                    // Process response here
//                    if (response[1] == 'A' && response[2] == 'K')
//                    {
//                        if (data[3] == response[3] &&
//                            data[4] == response[4])
//                        {
//                            Debug.WriteLine($"      GOOD Recieved CMD TYPE: {response[14]}");
//                        }
//                    }
//                    else if (response[1] == 'E' && response[2] == 'R')
//                    {
//                        // Handle error response
//#if DEBUG
//                        Debug.WriteLine("ERROR " + (char)response[1] + (char)response[2] + ":" + (char)response[3] + (char)response[4] + " " + string.Join(" , ", response));
//                        Debug.WriteLine(getError(response));
//#else
//                        Console.WriteLine("ERROR " + (char)response[1] + (char)response[2] + ":" + (char)response[3] + (char)response[4] + " " + string.Join(" , ", response));
//                        Console.WriteLine(getError(response));
//#endif
//                        //MessageBox.Show("Test Failed: \nTest received by PCB, but returned error. \n" +
//                        //"\nERROR: " + getError(response));
//                    }
//                    else if (response[1] == 'C' && response[2] == 'R')
//                    {
//                        // Handle error or unexpected response
//                        canResponseReceived = true;
//                        //return;
//                    }
//                    else
//                    {
//                        Debug.WriteLine("Waiting on acknowledgement response...");
//                    }
//                }
                await Task.Delay(100, cancellationToken);
                //MessageBox.Show($"Received {count} bytes");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending/receiving HID data: {ex.Message}");
                //MessageBox.Show($"Error sending/receiving HID data: {ex.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllBTN_Click(object sender, RoutedEventArgs e)
        {
            SelectAllTests();
        }

        private void SelectAllTests()
        {
            fromSelectBTN = true;
            if ((string)SelectAllBTN.Content == "Deselect All")
            {
                testsPannel.Children.OfType<ToggleButton>().ToList().ForEach(t => t.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF3767B3"));
                testsPannel.Children.OfType<ToggleButton>().ToList().ForEach(t => t.IsChecked = false);

            }
            else
            {
                testsPannel.Children.OfType<ToggleButton>().ToList().ForEach(t => t.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF559CE4"));
                testsPannel.Children.OfType<ToggleButton>().ToList().ForEach(t => t.IsChecked = true);
            }


            procedure.selectedTests.Clear();

            foreach (ToggleButton t in testsPannel.Children) {
                procedure.selectedTests.Add(t.IsChecked ?? false);
            }

            SelectAllBTN.Content = ((string)SelectAllBTN.Content == "Select All") ? "Deselect All" : "Select All";

            fromSelectBTN = false;
        }

        private void PrintPDFBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// toggle button click; select/deselect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testSelect(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleBtn && !fromSelectBTN)
            {
                int index = testsPannel.Children.IndexOf((UIElement)sender);
                bool isChecked = toggleBtn.IsChecked ?? false;
                Debug.WriteLine("Index: " + index);
                // Update UI
                toggleBtn.BorderBrush = isChecked 
                    ? (SolidColorBrush)new BrushConverter().ConvertFrom("#FF559CE4")
                    : (SolidColorBrush)new BrushConverter().ConvertFrom("#FF3767B3");

                // Update selection state
                procedure.UpdateTestSelection(index, isChecked);
                priorColor = (SolidColorBrush)toggleBtn.BorderBrush;
            }
        }

        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseOverTestItem(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                priorColor = (SolidColorBrush)toggleButton.BorderBrush;
                toggleButton.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF75AAFF");
            }
        }
        private void MouseExitTestItem(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                toggleButton.BorderBrush = priorColor;
            }
        }

        /// <summary>
        /// OPTIMIZED FAULT-BASED TEST EXECUTION
        /// Handles tests that monitor for fault occurrence or absence without interference from AI checking
        /// </summary>
        private Task<int> ExecuteFaultBasedTest(int phaseIDX, int lastIDX, int currentTestID, bool isLastPhase, CancellationToken cancellationToken)
        {
            TestPhase currentPhase = procedure.selectedFunc_List[phaseIDX];
            byte faultID = currentPhase.associatedFault;
            int fltIDX = faultID & 0x0F;
            int fltListNum = (faultID >> 4) - 1;

            // Get the specific fault we're monitoring
            FaultResponse associatedFault = MasterFaultList[fltListNum][fltIDX];

            // Check if fault state matches expected result
            if (FaultOccurrence && associatedFault.bitNUM == fltIDX)
            {
                if (associatedFault.status && currentPhase.expectedFaultResponse)  // BOTH TRUE PASS
                {
                    #if DEBUG
                        Debug.WriteLine($"  !!! PASSED {currentPhase.name} !!!");
                        Debug.WriteLine($"      Expected a fault recieved a fault");
                        Debug.WriteLine($"      Associated Fault: {associatedFault.faultID}");
                    #endif
                    // Test passed - fault state matches expectation
                    currentPhase.result = true;
                    hasPassed = true;
                    procedure.selectedFunc_List[phaseIDX] = currentPhase;
                    UpdateTestStatus(isLastPhase, currentPhase.parentID, currentPhase.result, true);
                    return Task.FromResult(phaseIDX + 1); // Move to next phase
                }
                else if (associatedFault.status && !currentPhase.expectedFaultResponse) // FAULT OCCURRED WHEN IT SHOULD NOT -> FAIL
                {
                    #if DEBUG
                        Debug.WriteLine($"  !!! FAILED {currentPhase.name} !!!");
                        Debug.WriteLine($"      Recieved fault when no fault was expected");
                        Debug.WriteLine($"      Associated Fault: {associatedFault.faultID}");
                    #endif
                    // Test failed - fault state doesn't match expectation
                    currentPhase.result = false;
                    hasFailed = true;
                    procedure.selectedFunc_List[phaseIDX] = currentPhase;
                    UpdateTestStatus(isLastPhase, currentPhase.parentID, currentPhase.result, true);
                    return Task.FromResult(phaseIDX + 1); // Move to next phase
                }
            }            
            
            // Check for timeout
            if (phaseTimer > currentPhase.phaseTimeLimit)
            {
                if (!associatedFault.status && !currentPhase.expectedFaultResponse)  // NO FAULT OCCURRED WHEN IT SHOULD NOT -> PASS
                {
                    // Timeout without fault when no fault was expected - PASS
                    #if DEBUG
                        Debug.WriteLine($"  !!! PASSED {currentPhase.name} !!!");
                        Debug.WriteLine($"      No fault recieved as expected");
                        Debug.WriteLine($"      Associated Fault: {associatedFault.faultID}");
                    #endif
                    currentPhase.result = true;
                    hasPassed = true;
                }
                else if (!associatedFault.status && currentPhase.expectedFaultResponse)  // NO FAULT OCCURRED WHEN IT SHOULD HAVE -> FAIL
                {
                    // Timeout without fault when fault was expected - FAIL
                    #if DEBUG
                        Debug.WriteLine($"  !!! FAILED {currentPhase.name} !!!");
                        Debug.WriteLine($"      Expected a fault none were recieved");
                        Debug.WriteLine($"      Associated Fault: {associatedFault.faultID}");
                    #endif
                    currentPhase.result = false;
                    hasFailed = true;
                }

                procedure.selectedFunc_List[phaseIDX] = currentPhase;
                UpdateTestStatus(isLastPhase, currentPhase.parentID, currentPhase.result, true);
                return Task.FromResult(phaseIDX + 1); // Move to next phase
            }

            // Continue monitoring - increment timer and stay on same phase
            return Task.FromResult(phaseIDX);
        }

        /// <summary>
        /// OPTIMIZED ANALOG INPUT TEST EXECUTION  
        /// Continuously monitors analog inputs for expected values without fault interference
        /// </summary>
        private Task<int> ExecuteAnalogInputTest(int phaseIDX, bool isLastPhase, CancellationToken cancellationToken)
        {
            TestPhase currentPhase = procedure.selectedFunc_List[phaseIDX];

            // Update all AI test results based on current vs expected values
            // Check if the specific AI index for this test matches expected value
            bool aiTestPassed = UpdateAllAnalogInputResults(currentPhase);
            

            // Check for timeout
            if (phaseTimer > currentPhase.phaseTimeLimit)
            {
                if (aiTestPassed)
                {
                    // AI value matches expected - test passes immediately
                    currentPhase.result = true;
                    hasPassed = true;
                    //Debug.WriteLine($"!!!!!!!!!! PHASE PASS TEST = {AI_TestResults[currentPhase.expectedAnalogs[]} Expected: {currentPhase.expectedAnalogVal} Recieved: {AI_Vals[aiIndex]} !!!!!!!!!!");
                    procedure.selectedFunc_List[phaseIDX] = currentPhase;
                    UpdateTestStatus(isLastPhase, currentPhase.parentID, currentPhase.result, true);
                    return Task.FromResult(phaseIDX + 1); // Move to next phase
                }
                else
                {
                    // Timeout without matching AI value - test fails
                    currentPhase.result = false;
                    hasFailed = true;
                    //Debug.WriteLine($"!!!!!!!!!! PHASE FAIL TEST = {AI_TestResults[aiIndex]} Expected: {currentPhase.expectedAnalogVal} Recieved: {AI_Vals[aiIndex]} !!!!!!!!!!");
                    procedure.selectedFunc_List[phaseIDX] = currentPhase;
                    UpdateTestStatus(isLastPhase, currentPhase.parentID, currentPhase.result, true);
                    return Task.FromResult(phaseIDX + 1); // Move to next phase
                }
            }
            // Continue monitoring - increment timer and stay on same phase
            //phaseTimer++;
            return Task.FromResult(phaseIDX);
        }

        /// <summary>
        /// OPTIMIZED ANALOG INPUT MONITORING
        /// Updates all AI test results continuously - more efficient than checking one at a time
        /// </summary>
        private bool UpdateAllAnalogInputResults(TestPhase _currentPhase)
        {
            bool allPassed = true;
            int passedcount = 0;
            for (int i = 0; i < _currentPhase.expectedAnalogs.Count; i++)
            {
                // Update the AI_TestResults dictionary for each expected analog value
                AI_TestResults[_currentPhase.expectedAnalogs[i].analogIDX] = 
                    (AI_Vals[_currentPhase.expectedAnalogs[i].analogIDX] == _currentPhase.expectedAnalogs[i].expectedVoltage);
                // Check if the current AI value matches the expected value
                passedcount = AI_TestResults[_currentPhase.expectedAnalogs[i].analogIDX] ? passedcount + 1 : passedcount;
                allPassed = (passedcount == _currentPhase.expectedAnalogs.Count);
            }
            //Debug.WriteLine($"{passedcount} / {_currentPhase.expectedAnalogs.Count}");
            return allPassed;
        }
    }
}




