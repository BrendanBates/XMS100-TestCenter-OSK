using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using TestCenter.TestDefinitions;
using static TestCenter.App;
using static TestCenter.TestProcedures.Procedure;

namespace TestCenter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public struct Header() // Header struct for test packet headers
        {
            public byte reportID = 0;
            public ushort validation = 0x5443; // 0x5443 = TC
            public byte testID = 4;
            public byte cmdQuantity = 11;
            public byte rsrvd1 = 0; // unused currently
            public byte rsrvd2 = 0; // unused currently
            public byte rsrvd3 = 0; // unused currently
        }
        Header header;

        public struct TestPKT()
        {
            public byte type = 0;
            public byte pin = 0xA1; // 0x5443 = TC
            public byte[]? data; // 2 bytes minimum of data for command or 12 bytes max for CAN message
        }
        TestPKT testPKT;

        public struct CAN_PKT()
        {
            public byte type = 0;
            public byte pin = 0xA1; // 0x5443 = TC
            public UInt32 id = 0; // CAN ID
            public byte[] data = new byte[8]; // 8 bytes of data for CAN message
        }

        public struct FullPKT()
        {
            public Header pktHeader = new Header();
            public List<TestPKT> cmdPKT = new List<TestPKT>();
        }
        FullPKT fullPKT;
        public struct ExpectedAnalog
        {
            public int expectedVoltage;
            public byte analogIDX; // Index of the analog input
            public ExpectedAnalog(int expectedVoltage, byte analogIDX)
            {
                this.expectedVoltage = expectedVoltage;
                this.analogIDX = analogIDX;
            }
        }

        public struct TestPhase
        {
            public int parentID;
            public Action phaseAction;      // The function to execute for this phase
            public int phaseTimeLimit;     // Individual timeout for this phase
            public bool expectedFaultResponse;   // true = pass when fault occurs, false = pass when no fault
            public byte associatedFault;       // What fault list indicator value should the system check
            public bool result;
            public byte testType;
            public string name;   
            public List<ExpectedAnalog> expectedAnalogs; // List of expected analog values for this phase

            public TestPhase(Action phaseAction, int faultTimeLimit, bool expectedFaultResponse, byte associatedFault, string name) // Constructor for fault tests
            {
                this.phaseAction = phaseAction;
                this.phaseTimeLimit = faultTimeLimit;
                this.expectedFaultResponse = expectedFaultResponse;
                this.associatedFault = associatedFault;
                this.expectedAnalogs = new List<ExpectedAnalog> { new ExpectedAnalog(0,0) };
                this.testType = CHECK_FAULT;
                this.name = name;
                this.result = false; // Initialize result to false for each phase
                this.parentID = 0; // Initialize parentID (will be set later in fillSelected)
            }
            public TestPhase(Action phaseAction, int phaseTimeLimit, List<ExpectedAnalog> expectedAnalogs, string name) // Overloaded constructor for analog input tests
            {
                this.phaseAction = phaseAction;
                this.phaseTimeLimit = phaseTimeLimit;
                this.expectedAnalogs = expectedAnalogs;
                this.testType = CHECK_AI;
                this.name = name;
                this.result = false; // Initialize result to false for each phase
                this.parentID = 0; // Initialize parentID (will be set later in fillSelected)
            }
            public TestPhase(Action phaseAction, int phaseTimeLimit, ExpectedAnalog expectedAnalog, string name) // Overloaded constructor for analog input tests
            {
                this.phaseAction = phaseAction;
                this.phaseTimeLimit = phaseTimeLimit;
                this.expectedAnalogs = new List<ExpectedAnalog> { expectedAnalog };
                this.testType = CHECK_AI;
                this.name = name;
                this.result = false; // Initialize result to false for each phase
                this.parentID = 0; // Initialize parentID (will be set later in fillSelected)
            }
        }

        public struct TestDictEntry
        {
            public int testID;
            public string name;
            public string description;
            public List<TestPhase> testPhases; //Changed from single Action to List<Action>
            public byte expectedFault;
            public bool result;
            public int currentPhase; // Track which phase we're on
            public int totalPhases; // Total number of phases
            public string causes; // Possible causes of fault

            public TestDictEntry(int testID, string name, string description, List<TestPhase> testPhases, byte expectedFault, bool result, string causes)
            {
                this.testID = testID;
                this.name = name;
                this.description = description;
                this.testPhases = testPhases;
                this.expectedFault = expectedFault;
                this.result = result;
                this.currentPhase = 0;
                this.totalPhases = testPhases?.Count ?? 1;
                this.causes = causes;
            }            // Constructor for single phase tests
            public TestDictEntry(int testID, string name, string description, TestPhase testPhase, byte expectedFault, bool result, string causes)
            {
                this.testID = testID;
                this.name = name;
                this.description = description;
                this.testPhases = new List<TestPhase> { testPhase };
                this.expectedFault = expectedFault;
                this.result = result;
                this.currentPhase = 0;
                this.totalPhases = 1;
                this.causes = causes;
            }

        }
        TestDictEntry testDictEntry;

        public struct FaultResponse
        {
            public bool status; // true or false, represents if fault has been triggered
            public string faultID;
            public string pinID;
            public byte pinNUM;
            public byte bitNUM;
            public string port;
            public string description; // Description of fault that has occured on board under test
            public string causes; // Possible causes of fault
            public FaultResponse(bool status, string faultID, string pinID, byte pinNUM, byte bitNUM, string port, string description, string causes)
            {
                this.status = status; // true or false, represents if fault has been triggered
                this.faultID = faultID;
                this.pinID = pinID;
                this.pinNUM = pinNUM;
                this.bitNUM = bitNUM;
                this.port = port;
                this.description = description; // Description of fault that has occured on board under test
                this.causes = causes; // Possible causes of fault
            }        
        }

        // MODULAR FAULT LISTS - Each board type can populate these
        public static FaultResponse[] Fault1_List { get; set; } = new FaultResponse[16]; // Increased from 15 to 16 for proper indexing
        public static FaultResponse[] Fault2_List { get; set; } = new FaultResponse[16]; // Increased from 15 to 16 for proper indexing  
        public static FaultResponse[] Fault3_List { get; set; } = new FaultResponse[16]; // Increased from 15 to 16 for proper indexing
        public static List<FaultResponse[]> MasterFaultList { get; set; } = new List<FaultResponse[]>();


        /// ANALOG INPUTS
        public const byte AI_01 = 29;
        public const byte AI_02 = 28;
        public const byte AI_03 = 27;
        public const byte AI_04 = 26;
        public const byte AI_05 = 25;
        public const byte AI_06 = 24;
        public const byte AI_07 = 23;
        public const byte AI_08 = 22;
        public const byte AI_09 = 15;
        public const byte AI_10 = 14;
        public const byte AI_11 = 13;
        public const byte AI_12 = 12;
        public const byte AI_13 = 11;
        public const byte AI_14 = 10;
        public const byte AI_15 = 9;
        public const byte AI_16 = 8;
        public const byte AI_17 = 0;
        public const byte AI_18 = 1;


        //public static List<TestPKT> resetPKTsList = new List<TestPKT>();
        public static bool IsResetListSet = false;
        public const byte CHECK_FAULT = 0;
        public const byte CHECK_AI = 1;
        public static byte testQuantity = 0;
        public const byte CanStart = 0x20; // CAN Write Start Command
        public const byte CanStop = 0x21; // CAN Write Stop Command
        public const byte CanSingle = 0x22; // CAN Write Single Command
        public const byte CanClear = 0x23; // Stop All CAN Commands
        public const int CmdPKT_Size = 18;
        public const long INTERFACE_BD1 = 610; // Interface Board ID when DI_DIP1 = 0
        public const long INTERFACE_BD2 = 866; // Interface Board ID when DI_DIP1 = 1
        public const long SRT_S_BD1 = 620; // SRT-S Board ID when DI_DIP1 = 0
        public const long SRT_S_BD2 = 876; // SRT-S Board ID when DI_DIP2 = 0
        public const long CSW1_S_1 = 616; // CAN Switch 1 Status ID when DI_DIP1 = 0
        public const long CSW1_S_2 = 872; // CAN Switch 1 Status ID when DI_DIP1 = 1
        public const long CSW2_S_1 = 618; // CAN Switch 2 Status ID when DI_DIP1 = 0
        public const long CSW2_S_2 = 874; // CAN Switch 2 Status ID when DI_DIP1 = 1
        public const long CSW1_Mess = 617; // CAN Switch 1 Message ID
        public const byte STATUS = 200; // Status byte for CAN messages
        public const byte FAULT = 0xFF; // Fault byte for CAN messages
        public static uint checknumber = 0;
        public static int[] AI_Vals { get; set; } = new int[50];
        public static int[] expectedAI_Vals { get; set; } = new int[50];
        public static bool[] AI_TestResults{ get; set; } = new bool[50];
        public static uint deviceID { get; set; } = 0x00; // Default to INT device ID
        public static long StatusID_1 = 0;
        public static long StatusID_2 = 0;
        public static long AnalogVI_ID1 = 0;
        public static long AnalogVI_ID2 = 0;
        public static long AnalogAI_ID1 = 0;
        public static long AnalogAI_ID2 = 0;
        public static long AnalogVO_ID1 = 0;
        public static long AnalogVO_ID2 = 0;
        public static long AnalogAO_ID1 = 0;
        public static long AnalogAO_ID2 = 0;
        public static long Analog28V_ID1 = 0;
        public static long Analog28V_ID2 = 0;

        public static List<TestPKT> tests_List = new List<TestPKT>();
        public static  List<TestPKT> resetList = new List<TestPKT>();

        public static void fillAllFuncList(uint _deviceID)
        {
            switch (_deviceID)
            {
                case 0x00: // Device ID for INT
                    fillINTFuncList();
                    break;
                case 0x01:
                    fillHzFuncList(); // Placeholder for another device type
                    break;
                case 0x02:
                    fillSRTFuncList(); // Placeholder for another device type
                    break;
                case 0x03:
                    fillBUSFuncList(); // Placeholder for another device type
                    break;
                default:
                    break;
            }

        }


        private static void fillINTFuncList()
        {
            // CRITICAL FIX: Clear tests_List to prevent accumulation between runs
            tests_List.Clear();
            resetList.Clear();

            var intDefinitions = new INT_Definitions(); // Create an instance of INT_Definitions  
            resetList = intDefinitions.reset();
            tests_List = INT_Definitions.intTests_List;
            StatusID_1 = INTERFACE_BD1;
            StatusID_2 = INTERFACE_BD2;
            AnalogAI_ID1 = 625;
            AnalogAI_ID2 = 881;
            AnalogAO_ID1 = 601;
            AnalogAO_ID2 = 857;
            AnalogVI_ID1 = 624;
            AnalogVI_ID2 = 880;
            AnalogVO_ID1 = 600;
            AnalogVO_ID2 = 856;
            Analog28V_ID1 = 603;
            Analog28V_ID2 = 859;
        }

        private static void fillSRTFuncList()
        {
            var srtDefinitions = new SRT_Definitions(); // Create an instance of SRT_Definitions  
            resetList = srtDefinitions.reset();
            tests_List = SRT_Definitions.srtTests_List;
            StatusID_1 = SRT_S_BD1;
            StatusID_2 = SRT_S_BD2;
        }

        private static void fillHzFuncList()
        {
            var HzDefinitions = new HZ_Definitions(); // Create an instance of HZ_Definitions  
            resetList = HzDefinitions.reset();
            tests_List = HzDefinitions.hzTests_List;
        }

        private static void fillBUSFuncList()
        {
            var busDefinitions = new BUS_Definitions(); // Create an instance of BUS_Definitions  
            resetList = busDefinitions.reset();
            tests_List = busDefinitions.busTests_List;
        }

    

        public static byte[] StatusIntsToBytes(ushort status1, ushort status2, ushort status3, ushort status4)
        {
            byte[] bytes = new byte[8];  // Create byte array
            Span<byte> span = bytes;     // Create span pointing to same memory of byte array

            // Each TryWriteBytes writes 2 bytes (Int16 = 16 bits = 2 bytes)
            BitConverter.TryWriteBytes(span.Slice(0, 2), status1); // Writes to bytes[0] and bytes[1]
            BitConverter.TryWriteBytes(span.Slice(2, 2), status2); // Writes to bytes[2] and bytes[3]
            BitConverter.TryWriteBytes(span.Slice(4, 2), status3); // Writes to bytes[4] and bytes[5]
            BitConverter.TryWriteBytes(span.Slice(6, 2), status4); // Writes to bytes[6] and bytes[7]
            //                                    ^  ^
            //                       Starting Index  Amount of Bytes
            return bytes; // Returns the modified array
        }

    }

}
