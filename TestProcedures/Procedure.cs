using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.RightsManagement;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Controls;
using TestCenter.TestDefinitions;
using System.Collections;
using System.Diagnostics;
using static TestCenter.App;
using TestCenter.TestUI;
using System.ComponentModel;

namespace TestCenter.TestProcedures
 {
     class Procedure
     {
        private const byte pwmRD = 0x41; //this is a temp val represents the number of tests needed to validate the interface board
        public const byte CMDS_IN_PKT = 14;
        public const byte CMD_SIZE = 4;
        public const byte MAX_NUM_CMDS = 64;
        private DefinitionsCommon common = new DefinitionsCommon();
        //private Definitions definitions = new Definitions();
        public List<TestPhase> selectedFunc_List = new List<TestPhase>();
        public List<FaultResponse> selectedTestResponse = new List<FaultResponse>();
        public List<TestDictEntry> selectedTestEntries = new List<TestDictEntry>();
        public byte[] fullPKT_array;
        //public byte[] header = new byte[9];
        public const byte genericPKT = 0;
        public const byte canPKT = 0x43;
        public List<bool> selectedTests = new List<bool>();
        private byte testCNT = 0;
        private int lastCMD = 0;
        private int testIDX = 0;

        FullPKT fullPKT = new FullPKT();
    




        /// fillSelected _________________________________________________________________________
        /// Description:  
        ///     Rebuilds the list of selected test functions based on the current selection state.
        ///     Selection state being determined by the selected toggle buttons.
        /// 
        /// Parameters: (none)
        /// 
        /// Reasoning:
        ///     Keeps the list of functions to execute in sync with the user's current test 
        ///     selections.
        /// --------------------------------------------------------------------------------------
        public void fillSelected()
        {
            selectedFunc_List.Clear(); // Clear last runs selection
            selectedTestResponse.Clear();
            selectedTestEntries.Clear();
            byte faultID;
            int fltIDX;
            int fltListNum;
            for (byte ii = 0; ii < TestUIs.testDictionary.Count; ii++) { // Loop through all of the tests 
                
                if (selectedTests[ii])
                {
                    var testEntry = TestUIs.testDictionary[ii];
                    faultID = TestUIs.testDictionary[ii].expectedFault; // Gets the expected fault location data

                    if (faultID != 0)
                    {
                        fltIDX = faultID & 0x0F; // Sets the fault index to be tested to the expected faults index
                        fltListNum = (faultID >> 4) - 1; // Sets the faultList to be tested based on the expected faults faultList location
                        // Add ALL phases for this test
                        selectedTestResponse.Add(MasterFaultList[fltListNum][fltIDX]); // Add the test entry located at the test index
                    }
                    testEntry.testID = ii;
                    selectedTestEntries.Add(testEntry); // Add the test entry located at the test index
                    for (int phaseIndex = 0; phaseIndex < testEntry.totalPhases; phaseIndex++)
                    {
                        TestPhase phase = testEntry.testPhases[phaseIndex];
                        phase.parentID = testEntry.testID; // Link phase under current test using parent tests testID

                        selectedFunc_List.Add(phase); // Add function located at current tests phase index
                    }
                }
            }
        }



        /// UpdateTestSelection __________________________________________________________________
        /// Description:  
        ///     Updates the selection state for a test and refreshes the selected function list.
        /// Parameters:
        ///     int index       - The index of the test to update.
        ///     bool isSelected - Whether the test is selected or not.
        /// Reasoning:
        ///     Allows dynamic updating of test selections and ensures the execution list is 
        ///     always current.
        /// --------------------------------------------------------------------------------------
        public void UpdateTestSelection(int index, bool isSelected)
        {
            if (index >= 0 && index < MAX_NUM_CMDS)
            {
                // Update selection array
                selectedTests[index] = isSelected;

                // Rebuild selected functions list
                fillSelected();
            }
        }



        /// runSelected _________________________________________________________________________
        /// Description:  
        ///     Executes all selected test functions, builds their packets, and returns the 
        ///     resulting byte arrays.
        /// Parameters: (none)
        /// Reasoning:
        ///     Automates the process of running all selected tests and collecting their 
        ///     communication packets for transmission.
        /// ------------------------------------------------------------------------------------        
        public List<byte[]> runSelected()
        {
            // CRITICAL FIX: Reset lastCMD to prevent indexing issues between sequential runs
            lastCMD = 0;
            testIDX = 0;
            
            int testQuantity = selectedFunc_List.Count; // testQuantity = the number of selected tests
            List<byte[]> selectedTestsArray = new List<byte[]>(); // Initialize array of selected test functions

            Debug.WriteLine($"=== runSelected DEBUG ===");
            Debug.WriteLine($"selectedFunc_List.Count: {selectedFunc_List.Count}");

            int functionIndex = 0;
            foreach (TestPhase phase in selectedFunc_List) // For every selected test function
            {
                int packetsBefore = selectedTestsArray.Count;

                phase.phaseAction(); // Execute the test function
                createFullPKT(selectedTestsArray, App.tests_List); // Creates packet to relay desired test command

                int packetsAfter = selectedTestsArray.Count;
                Debug.WriteLine($"Function {functionIndex}: Added {packetsAfter - packetsBefore} packets");

                //Debug.WriteLine(string.Join(" , ", selectedTestsArray[testIDX]) + "\nNUM CMDS: " + selectedTestsArray[testIDX].Length); // Display the selected test in the debug window
                functionIndex++;
            }

            Debug.WriteLine($"Total packets in selectedTestsArray: {selectedTestsArray.Count}");
            Debug.WriteLine($"========================");
            return selectedTestsArray;
        }


        public List<byte[]> getResetList(bool isResetSet)
        {
            List<byte[]> selectedTestsArray = new List<byte[]>(); // Initialize array of selected test functions
            if (isResetSet)
            {
                Debug.WriteLine($"RESET LIST COUNT = {App.resetList.Count()}");
                createFullPKT(selectedTestsArray, App.resetList); // Creates packet to relay desired test command
            }
            else
            {
                Debug.WriteLine($"RESET LIST COUNT = {App.resetList.Count()}");
                createFullPKT(selectedTestsArray, App.resetList); // Creates packet to relay desired test command
                                                                          //Debug.WriteLine(string.Join(" , ", selectedTestsArray[testIDX]) + "\nNUM CMDS: " + selectedTestsArray[testIDX].Length); // Display the selected test in the debug window
                                                                          //testIDX++; // Update testCNT for next selected test in array
               
                App.IsResetListSet = true;
            }
            return selectedTestsArray;
        }


        /// createFullPKT _______________________________________________________________________
        /// Description:  
        ///     Builds the header and command list for the current test and serializes the packet 
        ///     to a byte array.
        /// ------------------------------------------------------------------------------------
        private void createFullPKT(List<byte[]> _selectedTestsList, List<TestPKT> _testPKTs)
        {
            //string c = "";
            buildHeader(testCNT, _testPKTs); //headers takes bytes 0-7
            addTestPKTs(_selectedTestsList, _testPKTs); // fills the command list with as many commands as will fit in a single packet
            _selectedTestsList.Add(StructToArray(fullPKT));
            Debug.WriteLine($"RESULTING SIZE = {_selectedTestsList.Count()}");
            //Debug.WriteLine("Num CMDS: " + fullPKT.pktHeader.cmdQuantity + " TestCNT: " + fullPKT.pktHeader.testID);
            //Debug.WriteLine(string.Join(" , ", _selectedTestsList[testIDX]) + "\nNUM CMDS: " + _selectedTestsList[testIDX].Length); // Display the selected test in the debug window

            //c = fullPKT.pktHeader.ToString();
            //fullPKT.cmdPKT.ToArray();
            //fullPKT_array = fullPKT.cmdPKT.ToArray();
        }



        /// buildHeader _____________________________________________________________________________
        /// Description:  
        ///     Fills the full packets header with test and command information for the current test.
        /// Parameters:
        ///     byte _testCNT           - The test index or ID.
        ///     List<TestPKT> _testPKTs - The list of test packets to be included in the command.
        ///  Reasoning:
        ///     Ensures the header accurately reflects the current test and the number of 
        ///     commands to be sent.
        /// ------------------------------------------------------------------------------------------
        public void buildHeader(byte _testCNT, List<TestPKT> _testPKTs)
        {
            fullPKT.pktHeader.reportID = 0;
            fullPKT.pktHeader.validation = 0x5443; // 0x5443 = TC
            fullPKT.pktHeader.testID = _testCNT;
            fullPKT.pktHeader.cmdQuantity = (byte)_testPKTs.Count;
            fullPKT.pktHeader.rsrvd1 = 0;
            fullPKT.pktHeader.rsrvd2 = 0;
            fullPKT.pktHeader.rsrvd3 = 0;
            //Debug.WriteLine("Num CMDS: " + fullPKT.pktHeader.cmdQuantity + " TestCNT: " + fullPKT.pktHeader.testID);
            //fullPKT.pktHeader = header; // fill the header with the testID and cmdQuantity
        }



        /// addTestPKTs _________________________________________________________________________
        /// Description:  
        ///     Populates the command packet list with as many test packets as will fit in a 
        ///     64-byte frame.
        /// Parameters:
        ///     List<TestPKT> _testPKTs - The list of test packets to add to the command packet.
        /// Reasoning:
        ///     Ensures that only the number of commands that fit in a single packet are added, 
        ///     respecting packet size limits.
        /// ------------------------------------------------------------------------------------
        public void addTestPKTs(List<byte[]> _selectedTestsList,List<TestPKT> _testPKTs)
        {
            byte numcmds = 0;
            uint counter = 8; // header occupies bytes 0-7
            byte totalcmds = fullPKT.pktHeader.cmdQuantity;
            if (fullPKT.cmdPKT == null)
            {
                fullPKT.cmdPKT = new List<TestPKT>();
            }
            else
            {
                fullPKT.cmdPKT.Clear();
            }

            for (int i = lastCMD; i < _testPKTs.Count; i++)
            {
                counter = (_testPKTs[i].type >> 4 == 0x02 && _testPKTs[i].type != CanClear) ? //if the packet is any can message aside from Can Clear.
                                counter + 14 : // if test packet is a CAN message then add 14 to the byte counter
                                counter + 4;   // if test packet is a command type then only add 4 to the byte counter


                // if the current indexed test can fit in the current command (commands are comprised of 64 bytes)
                if (counter < 64)
                {
                    fullPKT.cmdPKT.Add(_testPKTs[i]);
                    lastCMD = i + 1; // save the last command that could be sent in the current test packet
                    //Debug.WriteLine($"TYPE: " + $"{_testPKTs[i].type:X}" + " PIN: " + _testPKTs[i].pin + " " + string.Join(" , ", _testPKTs[i].data) + " I=" + (i-numcmds) + " FullPKT CNT: " + fullPKT.cmdPKT.Count);
                    //Debug.WriteLine($"TYPE: " + $"{fullPKT.cmdPKT[i-numcmds].type:X}" + " PIN: " + fullPKT.cmdPKT[i-numcmds].pin + " " + string.Join(" , ", fullPKT.cmdPKT[i-numcmds].data) + " " + i + "\n");

                    if (lastCMD >= _testPKTs.Count) // if the current command is the last command needed to be sent
                        lastCMD = 0; // finished all
                }
                else
                {
                    _selectedTestsList.Add(StructToArray(fullPKT));
                    numcmds = (byte)(i); // Store number of command packets in last transmission. used as starting index of _testPKTs array to pull fullPKT.cmdPKTs from
                    //Debug.WriteLine("\nNum CMDS: " + fullPKT.pktHeader.cmdQuantity + " TestCNT: " + fullPKT.pktHeader.testID + " Index: " + numcmds);
                    fullPKT.cmdPKT.Clear(); // Clear fullPKT for next transmission
                    fullPKT.cmdPKT.Add(_testPKTs[i]); // Add the currently indexed testPKT to fullPKT of next transmission
                    //Debug.WriteLine($"TYPE: " + $"{_testPKTs[i].type:X}" + " PIN: " + _testPKTs[i].pin + " " + string.Join(" , ", _testPKTs[i].data) + " I=" + (i - numcmds) + " FullPKT CNT: " + fullPKT.cmdPKT.Count);
                    lastCMD = i + 1; // save the last command that could be sent in the current test packet

                    //// If no commands exist past what is currently indexed
                    if (lastCMD == _testPKTs.Count) {
                        fullPKT.pktHeader.cmdQuantity = 1; // Set cmdQuantity to 1
                    }
                    counter = 8;
                }
                fullPKT.pktHeader.cmdQuantity = (byte)(fullPKT.cmdPKT.Count); // Update the number of commands in the transmission
            }
            //fullPKT.pktHeader.cmdQuantity = (byte)(totalcmds - fullPKT.pktHeader.cmdQuantity);
        }



        /// StructToArray _______________________________________________________________________
        /// Description:  
        ///     Serializes a struct (including nested structs and lists) into a byte array.
        /// Parameters:
        ///     T obj - The struct to serialize.
        /// Reasoning:
        ///     Converts complex data structures into a format suitable for transmission and 
        ///     storage.
        /// ------------------------------------------------------------------------------------
        private static readonly Dictionary<Type, System.Reflection.FieldInfo[]> FieldCache = new Dictionary<Type, System.Reflection.FieldInfo[]>();

        public static byte[] StructToArray<T>(T obj) where T : struct
        {
            List<byte> result = new List<byte>();

            // Cache reflection results for fields
            Type type = typeof(T);
            if (!FieldCache.TryGetValue(type, out var fields))
            {
                fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                FieldCache[type] = fields;
            }

            // Serialize all fields of the struct dynamically
            foreach (var field in fields)
            {
                var value = field.GetValue(obj);

                if (value is byte b)
                {
                    result.Add(b);
                }
                else if (value is ushort s)
                {
                    result.AddRange(BitConverter.GetBytes(s));
                }
                else if (value is int i)
                {
                    result.AddRange(BitConverter.GetBytes(i));
                }
                else if (value is long l)
                {
                    result.AddRange(BitConverter.GetBytes(l));
                }
                else if (value is float f)
                {
                    result.AddRange(BitConverter.GetBytes(f));
                }
                else if (value is double d)
                {
                    result.AddRange(BitConverter.GetBytes(d));
                }
                else if (value is byte[] byteArray)
                {
                    result.AddRange(byteArray);
                }
                else if (field.FieldType == typeof(Header)) // Handle nested structs
                {
                    var h = (Header)value;
                    result.AddRange(StructToArray(h));
                }
                else if (value is IEnumerable<TestPKT> list) // Handle List<TestPKT>
                {
                    foreach (var item in list)
                    {
                        result.AddRange(StructToArray(item));
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported field type: {field.FieldType}");
                }
            }
            return result.ToArray();
        }
    }
}

