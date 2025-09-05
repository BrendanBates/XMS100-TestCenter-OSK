using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestCenter.TestUI;
using static TestCenter.App;
using static TestCenter.TestDefinitions.DefinitionsCommon;
using static TestCenter.TestProcedures.Procedure;

namespace TestCenter.TestDefinitions
{
    class INT_Definitions
    {
        /* DUPLICATION ANALYSIS AND CONSOLIDATION COMPLETED
         * ===================================================
         * 
         * RESOLVED DUPLICATIONS:
         * 1. ✅ Preserved modular MasterFaultList design - Each board type can populate their fault arrays
         * 2. ✅ Standardized fault descriptions between MasterFaultList and testdictionary 
         * 3. ✅ Removed all unused commented test entries from TestUIs display functions
         * 4. ✅ Fixed getTestResults() to properly access modular MasterFaultList instead of unified registry
         * 5. ✅ Increased fault array sizes from 15 to 16 for proper bit indexing (0-15)
         * 
         * ELIMINATED UNNECESSARY ELEMENTS:
         * 1. ✅ Removed ~200 lines of commented-out test dictionary entries in TestUIs.cs
         * 2. ✅ Fixed typo in fault description (changed "overvoltage" to correct "undervoltage" for UV faults)
         * 3. ✅ Fixed typo in fault description (changed "recieved" to "received")
         * 4. ✅ Fixed incorrect copy-paste description for IGBT C Fault (was showing "IGBT B")
         * 
         * PRESERVED MODULARITY:
         * - INT_Definitions.fillFLT_Lists() populates MasterFaultList for Interface board
         * - SRT_Definitions can populate their own fault lists when implemented
         * - BUS_Definitions can populate their own fault lists when implemented
         * - Each board type maintains their own fault definitions
         * 
         * FAULT CONSTANTS USAGE:
         * - Fault constants (INT_F_*) are used in test phase definitions to specify expected faults
         * - They map to specific positions in the MasterFaultList arrays
         * - This maintains clean separation between fault codes and fault descriptions
         * 
         * REMAINING WORK:
         * - SRT/BUS/Hz test implementations can follow same modular pattern
         * - Consider creating helper methods for fault code calculations if needed
         */

        public static List<TestPKT> intTests_List = new List<TestPKT>();

        // INTERFACE BOARD FAULT1_LIST
        private const byte INT_F_InputAHigh = 0X11;
        private const byte INT_F_InputBHigh = 0X12;
        private const byte INT_F_InputCHigh = 0X13;
        private const byte INT_F_InputALow = 0X14;
        private const byte INT_F_InputBLow = 0X15;
        private const byte INT_F_InputCLow = 0X16;
        private const byte INT_F_INPUT_INBAL = 0X17;
        private const byte INT_F_IGBTA = 0X18;
        private const byte INT_F_IGBTB = 0X19;
        private const byte INT_F_IGBTC = 0X1A;
        private const byte INT_F_SRTCOMM = 0X1B;
        private const byte INT_F_INTLK = 0X1C;
        private const byte INT_F_BUSUP = 0X1D;
        private const byte INT_F_CONTFB = 0X1E;
        private const byte INT_F_BUSTEMP = 0X1F;
        // INTERFACE BOARD FAULT2_LIST
        private const byte INT_F_EF_INTLK = 0X20;
        private const byte INT_F_LOCKOUT = 0X21;
        private const byte INT_F_28VO_OV = 0X22;
        private const byte INT_F_28VO_UV = 0X23;
        private const byte INT_F_28AO_OL = 0X24;
        private const byte INT_F_FREQ_LOW = 0X25;
        private const byte INT_F_FREQ_HIGH = 0X26;
        private const byte INT_F_400VOA_OV = 0X28;
        private const byte INT_F_400VOB_OV = 0X29;
        private const byte INT_F_400VOC_OV = 0X2A;
        private const byte INT_F_400VOA_UV = 0X2B;
        private const byte INT_F_400VOB_UV = 0X2C;
        private const byte INT_F_400VOC_UV = 0X2D;
        private const byte INT_F_400VOA_OL = 0X2E;
        private const byte INT_F_400VOB_OL = 0X2F;
        // INTERFACE BOARD FAULT3_LIST
        private const byte INT_F_400VOC_OL = 0X30;
        private const byte INT_F_IGBTA_OT = 0X31;
        private const byte INT_F_IGBTB_OT = 0X32;
        private const byte INT_F_IGBTC_OT = 0X33;
        private const byte INT_F_XFMR_OT = 0X34;
        private const byte INT_F_AUX1_OT = 0X35;
        private const byte INT_F_AUX2_OT = 0X36;
        private const byte INT_F_STOP = 0X37;        public INT_Definitions()
        {
            fillFLT_Lists();
            TestUIs.InitTestDisplay(App.deviceID);
        }

        private static void fillFLT_Lists()
        {
            MasterFaultList.Add( new FaultResponse[]
            {               //    status,      faultID,                 pinID         pinNUM  bitNUM      port             description               causes
                new FaultResponse(false, "ERROR",                       "ERROR",         0,      0,      "ERROR",     "ERROR",                        ""),
                new FaultResponse(false, "Input Voltage A - High",      "E9",           95,      1,      "P8.1",      "High input voltage on phase A", "Check TP12-14 with nothing plugged into P8, verify no voltage."),
                new FaultResponse(false, "Input Voltage B - High",      "E8",           96,      2,      "P8.3",      "High input voltage on phase B", "Check TP12-14 with nothing plugged into P8, verify no voltage."),
                new FaultResponse(false, "Input Voltage C - High",      "B4",           97,      3,      "P8.5",      "High input voltage on phase C", "Check TP12-14 with nothing plugged into P8, verify no voltage."),
                new FaultResponse(false, "Input Voltage A - Low",       "E9",           95,      4,      "P8.1",      "Low input voltage on phase A",  "Check TP12-14 with nothing plugged into P8, verify no voltage."),
                new FaultResponse(false, "Input Voltage B - Low",       "E8",           96,      5,      "P8.3",      "Low input voltage on phase B",  "Check TP12-14 with nothing plugged into P8, verify no voltage."),
                new FaultResponse(false, "Input Voltage C - Low",       "B4",           97,      6,      "P8.5",      "Low input voltage on phase C",  "Check TP12-14 with nothing plugged into P8, verify no voltage."),
                new FaultResponse(false, "Input Imbalance",             "pin4",         0,       7,      "EXX42A",    "Imbalance on input",            "INT_F_INPUT_INBAL = putTimeout(1, iALM_INPUT_IMBALANCE, ((InputAmpAvg> 250.0f) && (DI_SW5==0) && (InputAmpAdelta>0.2f || InputAmpBdelta>0.2f || InputAmpCdelta>0.2f)), tm2000ms)"),  //Adjust later
                new FaultResponse(false, "IGBT A Fault",                "A2",           39,      8,      "J1.1",      "Fault on IGBT A",               "(DI_FAULT_1 == 1 && DI_FAULT_2 == 1)"),  
                new FaultResponse(false, "IGBT B Fault",                "F3",           36,      9,      "J1.37",     "Fault on IGBT B",               "(DI_FAULT_1 == 1 && DI_FAULT_3 == 1)"),  
                new FaultResponse(false, "IGBT C Fault",                "D15",          38,      10,     "J1.35",     "Fault on IGBT C",               "(DI_FAULT_2 == 1 && DI_FAULT_3 == 1)"),  
                new FaultResponse(false, "SRT Comms Fault",             "pin7",         0,       11,     "EXX42A",    "Communications with the Switch Relay Terminal board have failed",               "Static signal or no signal received by interface board, possible CAN bus issue check IC 11 & 12."),  
                new FaultResponse(false, "CNTR Interlock Fault",        "G7",           89,      12,     "P7.7",      "Connection interlock failure",  ""),  
                new FaultResponse(false, "BUS UP Fault",                "D4",           59,      13,     "P2.4",      "Bus Up Fault",  ""),  
                new FaultResponse(false, "CNTR Feed Back Fault",        "D12",          57,      14,     "P2.5",      "Contactor Feedback Fault",  "Feedback not received."),  
                new FaultResponse(false, "BUS Temp Fault",              "F1",           63,      15,     "P2.6",      "BUS Temperature sensor faulted",  "Bus temp signal not received or bus temp exceeded acceptable running temperature")  
            });

            MasterFaultList.Add(new FaultResponse[]
            {               //    status,      faultID,                 pinID         pinNUM  bitNUM      port                                  description                             causes
                new FaultResponse(false, "E&F Interlink Fault",         "G7",           89,      0,      "P7.7",                    "E and F out of spec or not received",               ""),
                new FaultResponse(false, "Lock Out",                    "E0 E1 E5",     0,       1,      "J1.14, J1.15, J1.2",      "Lock out condition",                     ""),
                new FaultResponse(false, "28V Over Voltage",            "E4",           75,      2,      "J1.25",                   "Output voltage over 28v threshold",                ""),
                new FaultResponse(false, "28V Under Voltage",           "E4",           75,      3,      "J1.25",                   "Output voltage under 28v min threshold",          ""),
                new FaultResponse(false, "28V Output Overload",         "E2",           73,      4,      "J1.26",                   "28V output overload, current above max threshold",  ""),
                new FaultResponse(false, "Frequency - Low",             "F2",           37,      5,      "J1.24",                   "400Hz frequency low",                               ""),
                new FaultResponse(false, "Frequency - High",            "F2",           37,      6,      "J1.24",                   "400Hz frequency high",                              ""),
                new FaultResponse(false, "ERROR",                       "ERROR",         0,      7,      "ERROR",                   "ERROR",                              ""),
                new FaultResponse(false, "400Hz VO A Overvolt",         "A7",           67,      8,      "J1.17",                   "Output overvoltage on phase A",                     ""),
                new FaultResponse(false, "400Hz VO B Overvolt",         "E3",           76,      9,      "J1.04",                   "Output overvoltage on phase B",                     ""),
                new FaultResponse(false, "400Hz VO C Overvolt",         "A6",           68,      10,     "J1.16",                   "Output overvoltage on phase C",                     ""),
                new FaultResponse(false, "400Hz VO A Undervolt",        "A7",           67,      11,     "J1.17",                   "Output undervoltage on phase A",                     ""),
                new FaultResponse(false, "400Hz VO B Undervolt",        "E3",           76,      12,     "J1.04",                   "Output undervoltage on phase B",                     ""),
                new FaultResponse(false, "400Hz VO C Undervolt",        "A6",           68,      13,     "J1.16",                   "Output undervoltage on phase C",                     ""),
                new FaultResponse(false, "400Hz AO A Overload",         "E5",           81,      14,     "J1.2",                    "Phase A output amps exceeded acceptable range",      ""),
                new FaultResponse(false, "400Hz AO B Overload",         "E1",           69,      15,     "J1.15",                   "Phase B output amps exceeded acceptable range",      "")
            });

            MasterFaultList.Add(new FaultResponse[]
            {               //    status,      faultID,                 pinID         pinNUM  bitNUM      port             description                                      causes
                new FaultResponse(false, "Output Amp C Overload",      "E0",           70,      0,      "J1.14",            "Phase C output amps exceeded acceptable range", ""),
                new FaultResponse(false, "IGBT A Overtemp Fault",      "A2",           39,      1,      "J1.1",             "IGBT A Over heated",                           ""),
                new FaultResponse(false, "IGBT B Overtemp Fault",      "F3",           36,      2,      "J1.37",            "IGBT B Over heated",                           ""),
                new FaultResponse(false, "IGBT C Overtemp Fault",      "D15",          38,      3,      "J1.35",            "IGBT C Over heated",                           ""),
                new FaultResponse(false, "Transformer Overtemp Fault", "A0 & E6",      0,       4,      "P5.1 & P5.3",      "Transformer overheated",                       ""),
                new FaultResponse(false, "Aux1 Overtemp Fault",        "B2",           99,      5,      "P6.1",             "Aux1 overheated",                              ""),
                new FaultResponse(false, "Aux2 Overtemp Fault",        "G13",          74,      6,      "P6.3",             "Aux2 overheated",                              ""),  //Adjust later
                new FaultResponse(false, "STOP Fault",                 "00",           00,      7,      "",                 "Stop message triggered",                       "")
            });

        }

        public static void fillTestDict()
        {
            int count = 0;
            TestUIs.testDictionary = new List<App.TestDictEntry> {
                new App.TestDictEntry(
                    count++,
                    "CONN Interlink",
                    "The test checks if the Interface board is responding to the contactor interlock signals. " +
                    "If the contactor is open for more than one second the interface board should report " +
                    "a contactor interlock fault over the CAN bus.",
                    INT_Definitions.TestCONN_INTLK(),
                    0x20,
                    false,
                    "Possible Causes Listed Most Likely to Least: \n" +
                    "   1. CAN Chip Failure IC11\n" +
                    "   2. Trace Failure Between P7.7 to Interface MCU Pin RG7\n" +
                    "   3. Tester DO_09 or CAN Failure"
                ),
                new App.TestDictEntry(
                    count++,
                    "IGBT #1 Overtemp",
                    "The test checks if the interface board reports an overtemp fault on IGBT C when the associated" +
                    "temperature sensor is tripped and ensurring the interface board doesn't erroneously trip the fault.",
                    INT_Definitions.TestIGBT_1_OT(),
                    0x33,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "IGBT #2 Overtemp",
                    "The test checks if the interface board reports an overtemp fault on IGBT B when the associated" +
                    "temperature sensor is tripped and ensurring the interface board doesn't erroneously trip the fault.",
                    INT_Definitions.TestIGBT_2_OT(),
                    0x32,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "IGBT #3 Overtemp",
                    "The test checks if the interface board reports an overtemp fault on IGBT A when the associated" +
                    "temperature sensor is tripped and ensurring the interface board doesn't erroneously trip the fault.",
                    INT_Definitions.TestIGBT_3_OT(),
                    0x31,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Transformer Overtemp",
                    "The test checks if the interface board faults, reporting the transformer is overtemp when the associated" +
                    "temperature sensor is tripped and ensurring the interface board doesn't erroneously trip the fault.",
                    INT_Definitions.TestXFMR_OT(),
                    0x34,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "E & F Interlock",
                    "Tests if the E & F interlock response is operating as expected.",
                    INT_Definitions.TestEF(),
                    0x20,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Contactor Status",
                    "Tests if the Contactor Feedback function on the interface board is working properly.",
                    INT_Definitions.TestCONT_FB(),
                    0x1E,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Bus Temp",
                    "The test checks if the interface board faults, reporting the bus is overtemp when the associated" +
                    "temperature sensor is tripped and ensurring the interface board doesn't erroneously trip the fault.",
                    INT_Definitions.TestBUSTemp(),
                    0x1F,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output A Overvoltage",
                    "The test checks if the interface board faults, reporting an overvoltage fault when voltage level on a phase" +
                    "exceeds the voltage limit of 125 V.",
                    INT_Definitions.Test400VOA_OV(),
                    0x28,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output B Overvoltage",
                    "The test checks if the interface board faults, reporting an overvoltage fault when voltage level on a phase" +
                    "exceeds the voltage limit of 125 V.",
                    INT_Definitions.Test400VOB_OV(),
                    0x29,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output C Overvoltage",
                    "The test checks if the interface board faults, reporting an overvoltage fault when voltage level on a phase" +
                    "exceeds the voltage limit of 125 V.",
                    INT_Definitions.Test400VOC_OV(),
                    0x2A,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output A Undervoltage",
                    "The test checks if the interface board faults, reporting an undervoltage fault when voltage level on phase A" +
                    "is below 100 V.",
                    INT_Definitions.Test400VOA_UV(),
                    0x2B,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output B Undervoltage",
                    "The test checks if the interface board faults, reporting an undervoltage fault when voltage level on phase B" +
                    "is below 100 V.",
                    INT_Definitions.Test400VOB_UV(),
                    0x2C,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output C Undervoltage",
                    "The test checks if the interface board faults, reporting an undervoltage fault when voltage level on phase C" +
                    "is below 100 V.",
                    INT_Definitions.Test400VOC_UV(),
                    0x2D,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output A Overload",
                    "The test checks if the interface board faults on current overload of channel A output",
                    INT_Definitions.TestAOA_OL(),
                    0x2E,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output B Overload",
                    "The test checks if the interface board faults on current overload of channel B output",
                    INT_Definitions.TestAOB_OL(),
                    0x2F,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "Output C Overload",
                    "The test checks if the interface board faults on current overload of channel C output",
                    INT_Definitions.TestAOC_OL(),
                    0x30,
                    false,
                    ""
                ),
                new App.TestDictEntry(
                    count++,
                    "28V Output Overvoltage",
                    "Output overvoltage on 28V output",
                    INT_Definitions.Test28V_OV(),
                    0x22,
                    false,
                    ""
                ),
                //new App.TestDictEntry(
                //    count++,
                //    "Bus Up", 
                //    "The test checks if the interface board is monitoring the bus up properly and faults when it doesn't recieve " +
                //    "the 15v signal ",             
                //    INT_Definitions.TestChargeBUSUP(),
                //    0x1D,
                //    false
                //),
                //new App.TestDictEntry(
                //    count++,
                //    "Current Imbalance",
                //    "The test checks if the interface board faults when there is a input current imbalance.",
                //    INT_Definitions.TestAIN_IMBAL(),
                //    0x17,
                //    false
                //),
            };
        }

        //private async Task runSetupTests(CancellationToken cancellationToken, HidStream _hidStream)
        //{
        //    bool cancelTask = cancellationToken.IsCancellationRequested;
        //    FaultResponse contactorFault = MasterFaultList[0][12];
        //    FaultResponse srtCommFault = MasterFaultList[0][11];
        //    FaultResponse busUpFault = MasterFaultList[0][13];
        //    //FaultResponse contactorFault = MasterFaultList[0][12];
        //    try
        //    {

        //        while (!cancelTask)
        //        {
        //            contactorFault = MasterFaultList[0][12];
        //            srtCommFault = MasterFaultList[0][11];
        //            busUpFault = MasterFaultList[0][13];
        //            if (contactorFault.status && srtCommFault.status)
        //            {

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}


        public List<TestPKT> reset()
        {
            intTests_List.Clear();
            intTests_List.Add(CAN_CLEAR);
            //SIMULATE WORKING SRT BOARD
            intTests_List.Add(SRT_RESET);
            //intTests_List.Add(SIM_CSW_GOOD);
            intTests_List.Add(DO_09_ON); // Close 400Hz Contactor clearing INT_F_INTLK fault
            intTests_List.Add(DO5_01_ON); // Put in Manual Mode
            //TURN OFF IGBT FAULTS
            intTests_List.Add(DO5_02_OFF); // Turn off IGBT A Fault
            intTests_List.Add(DO5_03_OFF); // Turn off IGBT B Fault
            intTests_List.Add(DO5_04_OFF); // Turn off IGBT C Fault
            intTests_List.Add(DO_01_ON); // Turn off IGBT A Overtemp Fault
            intTests_List.Add(DO_02_ON); // Turn off IGBT B Overtemp Fault
            intTests_List.Add(DO_03_ON); // Turn off IGBT C Overtemp Fault
            //TURN OFF OVERTEMP FAULTS
            intTests_List.Add(DO_04_ON); // XFMR Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(DO_05_ON); // XFMR Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(DO15_03_ON); // BUS Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(temp1_OFF);
            intTests_List.Add(temp2_OFF);
            intTests_List.Add(DO15_01_ON); // Clear Charge Bus Up Fault
            intTests_List.Add(DO15_02_ON); // Clear Contactor Status Fault
            intTests_List.Add(SRT_400_START);
            return intTests_List;
        }

        /// 400HZ UNDER VOLTAGE ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<TestPhase> Test400VOA_UV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test400VOA_UV_FAIL, 4000, true,  INT_F_400VOA_UV, "400VOA_UV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test400VOA_UV_PASS, 1000, false,  INT_F_400VOA_UV, "400VOA_UV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test400VOA_UV_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_01_OFF); // Put in Auto Mode
            intTests_List.Add(AO4_OFF);
        }
        public static void Test400VOA_UV_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_01_ON); // Put in MAN Mode
            //AO4_ON.data = new byte[] { 0, 10 };
            //intTests_List.Add(AO4_ON);
        }


        public static List<TestPhase> Test400VOB_UV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test400VOB_UV_FAIL, 4000, true,  INT_F_400VOB_UV, "400VOB_UV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test400VOB_UV_PASS, 1000, false,  INT_F_400VOB_UV, "400VOB_UV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test400VOB_UV_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_01_OFF); // Put in Auto Mode
            intTests_List.Add(AO5_OFF);
        }
        public static void Test400VOB_UV_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_01_ON); // Put in MAN Mode
            //AO5_ON.data = new byte[] { 0 , 10 };
            //intTests_List.Add(AO5_ON);
        }


        public static List<TestPhase> Test400VOC_UV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test400VOC_UV_FAIL, 4000, true,  INT_F_400VOC_UV, "400VOC_UV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test400VOC_UV_PASS, 1000, false,  INT_F_400VOC_UV, "400VOC_UV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test400VOC_UV_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_01_OFF); // Put in Auto Mode
            intTests_List.Add(AO6_OFF);
        }
        public static void Test400VOC_UV_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_01_ON); // Put in MAN Mode
            //AO6_ON.data = new byte[] { 0 , 10 };
            //intTests_List.Add(AO6_ON);
        }

        /// 28V OVERVOLTAGE
        public static List<TestPhase> Test28V_UV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test28V_UV_FAIL, 1000, true, INT_F_28VO_UV, "28V_UV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test28V_UV_PASS, 1000, false, INT_F_28VO_UV, "28V_UV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test28V_UV_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(SRT_28_START);
            intTests_List.Add(AO7_OFF);
        }
        public static void Test28V_UV_PASS()
        {
            intTests_List.Clear();
            AO7_ON.data = new byte[] { 0, 100 };
            intTests_List.Add(AO7_ON);
        }

        /// 400HZ OVER VOLTAGE ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<TestPhase> Test400VOA_OV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test400VOA_OV_FAIL, 2000, true,  INT_F_400VOA_OV, "400VOA_OV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test400VOA_OV_PASS, 1000, false,  INT_F_400VOA_OV, "400VOA_OV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test400VOA_OV_FAIL()
        {
            intTests_List.Clear();
            AO4_ON.data = new byte[] { 255, 255 };
            intTests_List.Add(AO4_ON);
        }
        public static void Test400VOA_OV_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO4_OFF);
        }


        public static List<TestPhase> Test400VOB_OV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test400VOB_OV_FAIL, 2000, true,  INT_F_400VOB_OV, "400VOB_OV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test400VOB_OV_PASS, 1000, false,  INT_F_400VOB_OV, "400VOB_OV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test400VOB_OV_FAIL()
        {
            intTests_List.Clear();
            AO5_ON.data = new byte[] { 255, 255 };
            intTests_List.Add(AO5_ON);
        }
        public static void Test400VOB_OV_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO5_OFF);
        }


        public static List<TestPhase> Test400VOC_OV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test400VOC_OV_FAIL, 2000, true,  INT_F_400VOC_OV, "400VOC_OV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test400VOC_OV_PASS, 1000, false,  INT_F_400VOC_OV, "400VOC_OV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test400VOC_OV_FAIL()
        {
            intTests_List.Clear();
            AO6_ON.data = new byte[] { 255, 255 };
            intTests_List.Add(AO6_ON);
        }
        public static void Test400VOC_OV_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO6_OFF);
        }

        /// 28V OVERVOLTAGE
        public static List<TestPhase> Test28V_OV()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test28V_OV_FAIL, 2000, true, INT_F_28VO_OV, "28V_OV_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test28V_OV_PASS, 1000, false, INT_F_28VO_OV, "28V_OV_PASS_PHASE"),  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test28V_OV_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(SRT_28_START);
            AO7_ON.data = new byte[] { 255, 255 };
            intTests_List.Add(AO7_ON);
        }
        public static void Test28V_OV_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO7_OFF);
        }


        public static List<TestPhase> TestXFMR_OT()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestXFMR_OT_FAIL, 1000, true, INT_F_XFMR_OT, "XFMR_OT_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestXFMR_OT_PASS1, 1000, false, INT_F_XFMR_OT, "XFMR_OT_PASS_PHASE_1"),  // Phase 2: Test if interface board doesn't fault unexpectedly
                new App.TestPhase(TestXFMR_OT_PASS2, 1000, false, INT_F_XFMR_OT, "XFMR_OT_PASS_PHASE_2")  // Phase 3: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestXFMR_OT_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_04_OFF); // XFMR Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(DO_05_OFF); // XFMR Overtemp Fault ON = no fault OFF = fault
        }
        public static void TestXFMR_OT_PASS1()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_04_ON); // XFMR Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(DO_05_ON); // XFMR Overtemp Fault ON = no fault OFF = fault
        }
        public static void TestXFMR_OT_PASS2()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_04_OFF); // XFMR Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(DO_05_ON);  // XFMR Overtemp Fault ON = no fault OFF = fault
        }


        public static List<TestPhase> TestAUX1_OT()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestAUX1_OT_FAIL, 1000, true,  INT_F_AUX1_OT, "AUX1_OT_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestAUX1_OT_PASS, 1000, false, INT_F_AUX1_OT, "AUX1_OT_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestAUX1_OT_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_06_OFF); // AUX 1 Overtemp Fault ON = no fault OFF = fault
        }
        public static void TestAUX1_OT_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_06_ON); // AUX 1 Overtemp Fault ON = no fault OFF = fault
        }



        public static List<TestPhase> TestAUX2_OT()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestAUX2_OT_FAIL, 1000, true,   INT_F_AUX2_OT, "AUX2_OT_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestAUX2_OT_PASS, 1000, false, INT_F_AUX2_OT, "AUX2_OT_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestAUX2_OT_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_07_OFF); // AUX 2 Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(DO_08_OFF); // AUX 2 Overtemp Fault ON = no fault OFF = fault
        }
        public static void TestAUX2_OT_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_07_ON); // AUX 2 Overtemp Fault ON = no fault OFF = fault
            intTests_List.Add(DO_08_ON); // AUX 2 Overtemp Fault ON = no fault OFF = fault
        }


        public static List<TestPhase> TestAOA_OL()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestAOA_OL_FAIL, 1000, true,  INT_F_400VOA_OL, "AOA_OL_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestAOA_OL_PASS, 1000, false, INT_F_400VOA_OL, "AOA_OL_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestAOA_OL_FAIL()
        {
            intTests_List.Clear();
            AO1_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO1_ON);
        }
        public static void TestAOA_OL_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO1_OFF);
        }


        public static List<TestPhase> TestAOB_OL()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestAOB_OL_FAIL, 1000, true,  INT_F_400VOB_OL, "AOB_OL_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestAOB_OL_PASS, 1000, false, INT_F_400VOB_OL, "AOB_OL_PASS_PHASE") // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestAOB_OL_FAIL()
        {
            intTests_List.Clear();
            AO2_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO2_ON);
        }
        public static void TestAOB_OL_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO2_OFF);
        }



        public static List<TestPhase> TestAOC_OL()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestAOC_OL_FAIL, 1000, true,  INT_F_400VOC_OL, "AOC_OL_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestAOC_OL_PASS, 1000, false, INT_F_400VOC_OL, "AOC_OL_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestAOC_OL_FAIL()
        {
            intTests_List.Clear();
            AO3_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO3_ON);
        }
        public static void TestAOC_OL_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO3_OFF);
        }


        public static List<TestPhase> Test28AO_OL()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test28AO_OL_FAIL, 1000, true,  INT_F_28AO_OL, "28AO_OL_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(Test28AO_OL_PASS, 1000, false, INT_F_28AO_OL, "28AO_OL_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void Test28AO_OL_FAIL()
        {
            intTests_List.Clear();
            AO8_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO8_ON);
        }
        public static void Test28AO_OL_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(AO8_OFF);
        }


        public static List<TestPhase> TestAIN_IMBAL()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestAIN_IMBAL_FAIL, 2000, true,  INT_F_INPUT_INBAL, "AIN_IMBAL_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestAIN_IMBAL_PASS, 1000, false, INT_F_INPUT_INBAL, "AIN_IMBAL_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestAIN_IMBAL_FAIL()
        {
            intTests_List.Clear();
            AO13_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO13_ON);
            AO12_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO12_ON);
            AO14_ON.data = new byte[] { 0x00, 0x01 }; // Set amps out to 30
            intTests_List.Add(AO14_ON);
        }
        public static void TestAIN_IMBAL_PASS()
        {
            intTests_List.Clear();
            AO13_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO13_ON);
            AO12_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO12_ON);
            AO14_ON.data = new byte[] { 0xFF, 0xFF }; // Set amps out to 30
            intTests_List.Add(AO14_ON);
        }



        public static List<TestPhase> TestIGBT_1_OT()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestIGBT_1_OT_FAIL, 1000, true,  INT_F_IGBTC_OT, "IGBTC_OT_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestIGBT_1_OT_PASS, 1000, false, INT_F_IGBTC_OT, "IGBTC_OT_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestIGBT_1_OT_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_03_OFF); // Cause fault
        }
        public static void TestIGBT_1_OT_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_03_ON); // Stop fault
        }


        /// <summary>
        /// This will test IGBT C 
        /// first by setting the overtemperature signal low and checking for a fault response
        /// then the test will set the signal high and ensure no fault occurs.
        /// 
        /// If responses from the interface board don't match the behavior described above 
        /// the test will have failed.
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestIGBT_2_OT()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestIGBT_2_OT_FAIL, 1000,  true,  INT_F_IGBTB_OT, "IGBTB_OT_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestIGBT_2_OT_PASS, 1000, false, INT_F_IGBTB_OT, "IGBTB_OT_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestIGBT_2_OT_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_02_ON); // Cause fault
        }
        public static void TestIGBT_2_OT_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_02_OFF); // Cause fault
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestIGBT_3_OT()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestIGBT_3_OT_FAIL, 1000, true,  INT_F_IGBTA_OT, "IGBTA_OT_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestIGBT_3_OT_PASS, 1000, false, INT_F_IGBTA_OT, "IGBTA_OT_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestIGBT_3_OT_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_01_ON); // Cause fault
        }
        public static void TestIGBT_3_OT_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_01_OFF); // Cause fault
        }


        public static List<TestPhase> TestIGBT_1()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestIGBT_1_FAIL, 3000, true,  INT_F_IGBTC, "IGBTC_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestIGBT_1_PASS, 3000, false, INT_F_IGBTC, "IGBTC_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestIGBT_1_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_02_ON); // Cause fault
        }
        public static void TestIGBT_1_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_02_OFF); // Cause fault
        }




        public static List<TestPhase> TestIGBT_2()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestIGBT_2_FAIL, 1000, true,  INT_F_IGBTB, "IGBTB_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestIGBT_2_PASS, 1000, false, INT_F_IGBTB, "IGBTB_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestIGBT_2_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_03_ON); // Cause fault
        }
        public static void TestIGBT_2_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_03_OFF); // Cause fault
        }



        public static List<TestPhase> TestIGBT_3()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestIGBT_3_FAIL, 1000, true,  INT_F_IGBTA, "IGBTA_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestIGBT_3_PASS, 1000, false, INT_F_IGBTA, "IGBTA_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestIGBT_3_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_04_ON); // Cause fault
        }
        public static void TestIGBT_3_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO5_04_OFF); // Cause fault
        }



        public static List<TestPhase> TestCONT_FB()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestCONT_FB_FAIL, 3000, true,  INT_F_CONTFB, "CONT_FB_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestCONT_FB_PASS, 1000, false, INT_F_CONTFB, "CONT_FB_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestCONT_FB_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO15_02_OFF);
        }
        public static void TestCONT_FB_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO15_02_ON);
        }


        public static List<TestPhase> TestEF()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestEF_FAIL, 3000, true,  INT_F_EF_INTLK, "EF_INTLK_1"), // Phase 1: Test if faults when intended
            };
            return list;
        }
        public static void TestEF_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(SRT_EF_FAIL);
        }



        public static List<TestPhase> TestChargeBUSUP()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestChargeBUSUP_FAIL, 1000, true,  INT_F_BUSUP, "CHARGE_BUSUP_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestChargeBUSUP_PASS, 1000, false, INT_F_BUSUP, "CHARGE_BUSUP_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestChargeBUSUP_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO15_01_OFF);
        }
        public static void TestChargeBUSUP_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO15_01_ON);
        }


        public static List<TestPhase> TestBUSTemp()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestBUSTemp_FAIL, 4000, true,  INT_F_BUSTEMP, "BUS_TEMP_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestBUSTemp_PASS, 1000, false, INT_F_BUSTEMP, "BUS_TEMP_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestBUSTemp_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO15_03_OFF);
        }
        public static void TestBUSTemp_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO15_03_ON);
        }


        public static List<TestPhase> TestCONN_INTLK()
        {
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestCONN_INTLK_FAIL, 2000, true,  INT_F_INTLK, "INTERLINK_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestCONN_INTLK_PASS, 2000, false, INT_F_INTLK, "INTERLINK_PASS_PHASE")  // Phase 2: Test if interface board doesn't fault unexpectedly
            };
            return list;
        }
        public static void TestCONN_INTLK_FAIL()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_09_OFF); // Open 400Hz Contactor causing fault
        }
        public static void TestCONN_INTLK_PASS()
        {
            intTests_List.Clear();
            intTests_List.Add(DO_09_ON); // Close 400Hz Contactor preventing fault
        }
    }
}