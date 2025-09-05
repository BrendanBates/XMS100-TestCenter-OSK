using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCenter.TestUI;
using static TestCenter.App;
using static TestCenter.TestDefinitions.DefinitionsCommon;
using static TestCenter.TestProcedures.Procedure;

namespace TestCenter.TestDefinitions
{
    class SRT_Definitions
    {        
        public byte testQuantity = 0;
        //private List<TestPKT> tests_List = new List<TestPKT>();
        public static List<TestPKT> srtTests_List = new List<TestPKT>();
        
        // ANALOG INDEXES    
        private const byte IDX_HOIST_HU1 = AI_01;  
        private const byte IDX_HOIST_HD1 = AI_02; 
        private const byte IDX_SHUNT = AI_03;  
        private const byte IDX_FANS_HEAT = AI_04; // AN05 
        private const byte IDX_FANS_XFMR = AI_05; // AN06 
        private const byte IDX_28V_CONT = AI_06;  // AN07 
        private const byte IDX_OUT_CONT = AI_07;  // AN08 
        private const byte IDX_ESIG = AI_08;      // AN09 
        private const byte IDX_REM_FLT_NO = AI_09; // AN10 
        private const byte IDX_REM_FLT_NC = AI_10; // AN11 
        private const byte IDX_DOOR_INTLK = AI_11; // AN12 
        private const byte IDX_ESTOP = AI_12;  // AN13 
        private const byte IDX_REM1_NO = AI_13;  // AN13 
        private const byte IDX_REM1_NC = AI_14;  // AN13 
        private const byte IDX_REM2_NO = AI_15;  // AN13 
        private const byte IDX_REM2_NC = AI_16;  // AN13 
        private const byte IDX_REM_PWR_NO = AI_17;  // AN13 
        private const byte IDX_REM_PWR_NC = AI_18;  // AN13        // SRT Test Fault Constants (using INT board fault for SRT communication)
        private const byte SRT_F_SRTCOMM = 0X1B; // Uses INT_F_SRTCOMM since it's SRT-related

        public SRT_Definitions()
        {
            fillTestsDict();
            TestUIs.InitTestDisplay(App.deviceID);
        }

        public List<TestPKT> reset()
        {
            srtTests_List.Clear();
            srtTests_List.Add(CAN_CLEAR);
            srtTests_List.Add(temp1_OFF);
            srtTests_List.Add(temp2_OFF);
            //srtTests_List.Add(DO_01_OFF);
            //srtTests_List.Add(DO_02_OFF);
            //srtTests_List.Add(DO_03_OFF);
            //srtTests_List.Add(DO_04_OFF);
            //srtTests_List.Add(DO_05_OFF);
            //srtTests_List.Add(DO_06_OFF);
            //srtTests_List.Add(DO_07_OFF);
            //srtTests_List.Add(DO_08_OFF);
            //srtTests_List.Add(DO_09_OFF);
            //srtTests_List.Add(DO_10_OFF);
            //srtTests_List.Add(DO5_01_OFF);
            //srtTests_List.Add(DO5_02_OFF);
            //srtTests_List.Add(DO5_03_OFF);
            //srtTests_List.Add(DO5_04_OFF);
            //srtTests_List.Add(DO15_01_OFF);
            //srtTests_List.Add(DO15_02_OFF);
            //srtTests_List.Add(DO15_03_OFF);
            //srtTests_List.Add(DO15_04_OFF);
            //srtTests_List.Add(DO24_01_OFF);
            //srtTests_List.Add(DO24_02_OFF);
            //srtTests_List.Add(DO24_03_OFF);
            //srtTests_List.Add(DO24_04_OFF);
            return srtTests_List;
        }

        private void clearCAN()
        {
            srtTests_List.Clear();
            srtTests_List.Add(CAN_CLEAR);
        }


        /// <summary>
        /// Test SRT Hoist Up functionality - turns on DO15_01 and sends CSW_HOIST_UP CAN communication
        /// Checks that AI_01 reads 10V when CSW_HOIST_UP is active
        /// </summary>
        /// <returns>List of test phases for hoist up test</returns>
        public static List<TestPhase> TestSRTHoistUp()
        {
            List<ExpectedAnalog> phase1_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_HOIST_HD1),
                new ExpectedAnalog(4, IDX_HOIST_HU1),
            };
            List<ExpectedAnalog> phase3_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_HOIST_HD1),
                new ExpectedAnalog(0, IDX_HOIST_HU1),
            };
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestSRTHoistUp_ON, 40, phase1_AIs, "SRT_HOIST_UP_FAIL_PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestSRTHoistUp_OFF, 40, phase3_AIs, "SRT_HOIST_UP_PASS_PHASE"),  // Phase 2: Test AI_01 reads 0V when hoist up is not active
            };
            return list;
        }        
        public static void TestSRTHoistUp_ON()
        {
            // Turn on DO15_01 and send CSW_HOIST_UP CAN communication 
            // Should result in 0V on AI_01 (IDX_HOIST_HU1)
            srtTests_List.Clear();
            srtTests_List.Add(CSW_HOIST_UP); // Send Hoist up Command
            srtTests_List.Add(DO15_01_ON); // Turn on Hoist Control Output
            // Set expected analog input value for AI_01 to 4V
        }
        public static void TestSRTHoistUp_OFF()
        {
            // Turn off DO15_01 and send CSW_HOIST_UP CAN communication
            // Should result in 0V on AI_01 (IDX_HOIST_HU1)
            srtTests_List.Clear();
            srtTests_List.Add(CSW_HOIST_UP); // Send Hoist up Command
            srtTests_List.Add(DO15_01_OFF); // Turn off Hoist Control Output
            // Set expected analog input value for AI_01 to 0V
        }


        /// <summary>
        /// Test SRT Hoist Up functionality - turns on DO15_01 and sends CSW_HOIST_DOWN CAN communication
        /// Checks that AI_02 reads 4V when CSW_HOIST_DOWN is active
        /// </summary>
        /// <returns>List of test phases for hoist up test</returns>
        public static List<TestPhase> TestSRTHoistDown()
        {
            List<ExpectedAnalog> phase1_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(4, IDX_HOIST_HD1),
                new ExpectedAnalog(0, IDX_HOIST_HU1),
            };
            List<ExpectedAnalog> phase2_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_HOIST_HD1),
                new ExpectedAnalog(0, IDX_HOIST_HU1),
            };
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestSRTHoistDown_ON, 40, phase1_AIs, "SRT_HOIST_DOWN_4v Recieved _PHASE"), // Phase 1: Test if faults when intended
                new App.TestPhase(TestSRTHoistDown_OFF, 40, phase2_AIs, "SRT_HOIST_DOWN_0v Recieved_PHASE"),  // Phase 2: Test AI_02 reads 10V when hoist up is active
            };
            return list;
        }
        public static void TestSRTHoistDown_ON()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(CSW_HOIST_DOWN); // Send Hoist Down Command
            //srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(DO15_01_ON); // Turn on Hoist Control Output
        }
        public static void TestSRTHoistDown_OFF()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication
            // Should result in 4V on AI_02 (IDX_HOIST_HU1)
            srtTests_List.Clear();
            srtTests_List.Add(CSW_HOIST_DOWN); // Send Hoist Down Command
            //srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(DO15_01_OFF); // Turn off Hoist Control Output
            // Set expected analog input value for AI_02 to 4V
        }

        /// <summary>
        /// Test SRT Hoist Up functionality - turns on DO15_01 and sends CSW_HOIST_UP CAN communication
        /// Checks that AI_01 reads 10V when CSW_HOIST_UP is active
        /// </summary>
        /// <returns>List of test phases for hoist up test</returns>
        public static List<TestPhase> TestREM1()
        {
            int timelimit = 25;
            List<ExpectedAnalog> phase1_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(4, IDX_REM1_NO),
                new ExpectedAnalog(0, IDX_REM1_NC),
            };
            List<ExpectedAnalog> phase2_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_REM1_NO),
                new ExpectedAnalog(4, IDX_REM1_NC),
            };
            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestREM1_ON, timelimit, phase1_AIs, "REMOTE 1 ON PHASE"), // Phase 1: 
                new App.TestPhase(TestREM1_OFF, timelimit, phase2_AIs, "REMOTE 1 OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void TestREM1_ON()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(CSW_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(INT_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(DO15_02_ON); // Turn on REM Control Output
        }
        public static void TestREM1_OFF()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication
            // Should result in 4V on AI_02 (IDX_HOIST_HU1)
            srtTests_List.Clear();
            srtTests_List.Add(CSW_400_OFF); // Turn on 400Hz Command
            srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(DO15_02_ON); // Turn off REM Control Output 
        }




        /// <summary>
        /// Test SRT Hoist Up functionality - turns on DO15_01 and sends CSW_HOIST_UP CAN communication
        /// Checks that AI_01 reads 10V when CSW_HOIST_UP is active
        /// </summary>
        /// <returns>List of test phases for hoist up test</returns>
        public static List<TestPhase> TestREM2()
        {
            List<ExpectedAnalog> phase1_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(4, IDX_REM2_NO),
                new ExpectedAnalog(0, IDX_REM2_NC),
            };
            List<ExpectedAnalog> phase2_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_REM2_NO),
                new ExpectedAnalog(4, IDX_REM2_NC),
            };

            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestREM2_ON, 25, phase1_AIs, "REMOTE 2 ON PHASE"), // Phase 1: 
                new App.TestPhase(TestREM2_OFF, 50, phase2_AIs, "REMOTE 2 OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void TestREM2_ON()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(CSW_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(INT_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(DO15_03_ON); // Turn on REM Control Output
        }
        public static void TestREM2_OFF()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication
            // Should result in 4V on AI_02 (IDX_HOIST_HU1)
            srtTests_List.Clear();
            srtTests_List.Add(CSW_400_OFF); // Turn on 400Hz Command
            srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(DO15_03_ON); // Turn on REM Control Output
        }




        /// <summary>
        /// Test SRT Hoist Up functionality - turns on DO15_01 and sends CSW_HOIST_UP CAN communication
        /// Checks that AI_01 reads 10V when CSW_HOIST_UP is active
        /// </summary>
        /// <returns>List of test phases for hoist up test</returns>
        public static List<TestPhase> TestREM_PWR()
        {
            List<ExpectedAnalog> phase1_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(4, IDX_REM_PWR_NO),
                new ExpectedAnalog(0, IDX_REM_PWR_NC),
            };
            List<ExpectedAnalog> phase2_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_REM_PWR_NO),
                new ExpectedAnalog(0, IDX_REM_PWR_NC),
            };

            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestREM_PWR_ON, 40, phase1_AIs, "REMOTE 2 ON PHASE"), // Phase 1: 
                new App.TestPhase(TestREM_PWR_OFF, 40, phase2_AIs, "REMOTE 2 ON PHASE"), // Phase 1: 
            };
            return list;
        }
        public static void TestREM_PWR_ON()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(DO15_04_ON); // Turn on REM Control Output
        }
        public static void TestREM_PWR_OFF()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(DO15_04_OFF); // Turn on REM Control Output
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestREMOTE_FAULT()
        {
            int timelimit = 25;

            List<ExpectedAnalog> phase1_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(4, IDX_REM_FLT_NO),
                new ExpectedAnalog(0, IDX_REM_FLT_NC),
            };
            List<ExpectedAnalog> phase2_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_REM_FLT_NO),
                new ExpectedAnalog(4, IDX_REM_FLT_NC),
            };
            List<ExpectedAnalog> phase3_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_REM_FLT_NO),
                new ExpectedAnalog(0, IDX_REM_FLT_NC),
            };

            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestREM_FAULT, 100, phase1_AIs, "REMOTE 2 ON PHASE"), // Phase 1: 
                new App.TestPhase(TestREM_NO_FAULT, 40, phase2_AIs, "REMOTE 2 OFF PHASE"),  // Phase 2: 
                new App.TestPhase(TestREM_OFF, 40, phase3_AIs, "REMOTE 2 ON PHASE"), // Phase 1: 
            };
            return list;
        }
        public static void TestREM_FAULT()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(INT_S_FAULT); // Turn on Fault Command
            srtTests_List.Add(DO5_03_ON); // Turn on REM Control Output
        }
        public static void TestREM_NO_FAULT()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(INT_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(DO5_03_ON); // Turn on REM Control Output
        }
        public static void TestREM_OFF()
        {
            // Turn on DO15_01 and send CSW_HOIST_DOWN CAN communication 
            srtTests_List.Clear();
            srtTests_List.Add(DO5_03_OFF); // Turn on REM Control Output
        }




        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> Test28V_CONT()
        {
            int timelimit = 25;

            ExpectedAnalog AI_28V_ON = new ExpectedAnalog(4, IDX_28V_CONT);
            ExpectedAnalog AI_28V_OFF = new ExpectedAnalog(0, IDX_28V_CONT);


            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(Test28V_NOVOLT, timelimit, AI_28V_OFF, "REMOTE 2 OFF PHASE"),  // Phase 2: 
                new App.TestPhase(Test28V_ON, timelimit, AI_28V_ON, "REMOTE 2 ON PHASE"), // Phase 1: 
                new App.TestPhase(Test28V_OFF, timelimit*10, AI_28V_OFF, "REMOTE 2 OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void Test28V_NOVOLT()
        {
            // Tests for when the relay is in the on position and the voltage is off
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(INT_28V_ON); // Turn on 400Hz Command
            srtTests_List.Add(CSW_28V_ON); // Turn on 400Hz Command
            srtTests_List.Add(DO5_01_OFF); // Turn on REM Control Output
        }
        public static void Test28V_ON()
        {
            // Tests for when relay is in the on position and voltage should be recieved
            srtTests_List.Clear();
            //srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(INT_28V_ON); // Turn on 400Hz Command
            srtTests_List.Add(CSW_28V_ON); // Turn on 400Hz Command
            srtTests_List.Add(DO5_01_ON); // Turn on REM Control Output
        }
        public static void Test28V_OFF()
        {
            // Tests for when relay is in the off position and no voltage should get through
            srtTests_List.Clear();
            srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(CSW_CONT_OFF); // Turn on 400Hz Command
            srtTests_List.Add(DO5_01_ON); // Turn on REM Control Output
        }






        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestAUX_CONT()
        {
            int timelimit = 25;

            ExpectedAnalog AI_AUX_ON = new ExpectedAnalog(4, IDX_OUT_CONT);
            ExpectedAnalog AI_AUX_OFF = new ExpectedAnalog(0, IDX_OUT_CONT);


            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestAUX_NOVOLT, timelimit, AI_AUX_OFF, "REMOTE 2 OFF PHASE"),  // Phase 2: 
                new App.TestPhase(TestAUX_ON, timelimit, AI_AUX_ON, "REMOTE 2 ON PHASE"), // Phase 1: 
                new App.TestPhase(TestAUX_OFF, timelimit*12, AI_AUX_OFF, "REMOTE 2 OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void TestAUX_NOVOLT()
        {
            // Tests for when the relay is in the on position and the voltage is off
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(CSW_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(INT_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(DO5_02_OFF); // Turn on REM Control Output
        }
        public static void TestAUX_ON()
        {
            // Tests for when relay is in the on position and voltage should be recieved
            srtTests_List.Clear();
            //srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(CSW_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(INT_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(DO5_02_ON); // Turn on REM Control Output
        }
        public static void TestAUX_OFF()
        {
            // Tests for when the relay is in the off position and the voltage is on
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(CSW_400_OFF); // Turn on 400Hz Command
            srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(DO5_02_ON); // Turn on REM Control Output
        }






        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestESIG()
        {
            int timelimit = 25;

            ExpectedAnalog ESIG_ON = new ExpectedAnalog(4, IDX_ESIG);
            ExpectedAnalog ESIG_OFF = new ExpectedAnalog(0, IDX_ESIG);


            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestESIG_ON, timelimit, ESIG_ON, "ESIG ON PHASE"), // Phase 1: 
                new App.TestPhase(TestESIG_OFF, timelimit, ESIG_OFF, "ESIG OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void TestESIG_ON()
        {
            // Tests for when relay is in the on position and voltage should be recieved
            srtTests_List.Clear();
            srtTests_List.Add(INT_400_ON); // Turn on 400Hz Command
        }
        public static void TestESIG_OFF()
        {
            // Tests for when the relay is in the on position and the voltage is off
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
        }






        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestFSIG()
        {
            int timelimit = 25;

            //ExpectedAnalog FSIG_ON = new ExpectedAnalog(4, IDX_ESIG);
            //ExpectedAnalog FSIG_OFF = new ExpectedAnalog(0, IDX_ESIG);


            List<TestPhase> list = new List<TestPhase>{
                //new App.TestPhase(TestFSIG_ON, timelimit, FSIG_ON, "ESIG ON PHASE"), // Phase 1: 
                //new App.TestPhase(TestFSIG_OFF, timelimit, FSIG_OFF, "ESIG OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void TestFSIG_ON()
        {
            // Tests for when relay is in the on position and voltage should be recieved
            srtTests_List.Clear();
            //srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
            srtTests_List.Add(DO24_02_ON); // Turn on 400Hz Command
        }
        public static void TestFSIG_OFF()
        {
            // Tests for when the relay is in the on position and the voltage is off
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(DO24_02_OFF); // Turn on 400Hz Command
        }






        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestFANS_HTSNK()
        {
            int timelimit = 25;

            ExpectedAnalog HTSNK_ON = new ExpectedAnalog(4, IDX_FANS_HEAT);
            ExpectedAnalog HTSNK_OFF = new ExpectedAnalog(0, IDX_FANS_HEAT);


            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestFANS_HTSNK_ON, timelimit, HTSNK_ON, "REMOTE 2 ON PHASE"), // Phase 1: 
                new App.TestPhase(TestFANS_HTSNK_OFF, timelimit*75, HTSNK_OFF, "REMOTE 2 OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void TestFANS_HTSNK_ON()
        {
            // Tests for when relay is in the on position and voltage should be recieved
            srtTests_List.Clear();
            srtTests_List.Add(INT_400_ON); // Turn on 400Hz Command
            srtTests_List.Add(INT_AUX1_OT); // Turn on overtemp fault
        }
        public static void TestFANS_HTSNK_OFF()
        {
            // Tests for when the relay is in the on position and the voltage is off
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(INT_ALL_OFF); // Turn on overtemp fault
        }






        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestFANS_XFMR()
        {
            int timelimit = 25;

            ExpectedAnalog XFMR_ON = new ExpectedAnalog(4, IDX_FANS_XFMR);
            ExpectedAnalog XFMR_OFF = new ExpectedAnalog(0, IDX_FANS_XFMR);


            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestFANS_XFMR_ON, timelimit, XFMR_ON, "REMOTE 2 ON PHASE"), // Phase 1: 
                new App.TestPhase(TestFANS_XFMR_OFF, timelimit, XFMR_OFF, "REMOTE 2 OFF PHASE"),  // Phase 2: 
            };
            return list;
        }
        public static void TestFANS_XFMR_ON()
        {
            // Tests for when relay is in the on position and voltage should be recieved
            srtTests_List.Clear();
            srtTests_List.Add(INT_S_XFMR_T1); // Turn on 400Hz Command
        }
        public static void TestFANS_XFMR_OFF()
        {
            // Tests for when the relay is in the on position and the voltage is off
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(INT_ALL_OFF); // Turn on 400Hz Command
        }






        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<TestPhase> TestSHUNT()
        {
            int timelimit = 40;

            List<ExpectedAnalog> phase1_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(4, IDX_SHUNT),
                new ExpectedAnalog(4, IDX_DOOR_INTLK),
                new ExpectedAnalog(4, IDX_ESTOP),
            };
            List<ExpectedAnalog> phase2_AIs = new List<ExpectedAnalog> {
                new ExpectedAnalog(0, IDX_SHUNT),
                new ExpectedAnalog(0, IDX_DOOR_INTLK),
                new ExpectedAnalog(0, IDX_ESTOP),
            };


            List<TestPhase> list = new List<TestPhase>{
                new App.TestPhase(TestSHUNT_OFF, timelimit, phase2_AIs, "REMOTE 2 OFF PHASE"),  // Phase 2: 
                new App.TestPhase(TestSHUNT_ON, timelimit, phase1_AIs, "REMOTE 2 ON PHASE"), // Phase 1: 
            };
            return list;
        }
        public static void TestSHUNT_OFF()
        {
            // Tests for when the relay is in the on position and the voltage is off
            // Should result in 0v recieved.
            srtTests_List.Clear();
            srtTests_List.Add(DO24_04_ON); // Turn on remote ESTOP Flipping relay to ground the shunt
        }
        public static void TestSHUNT_ON()
        {
            // Tests for when relay is in the on position and voltage should be recieved
            srtTests_List.Clear();
            srtTests_List.Add(DO24_04_OFF); // Turn off remote ESTOP Flipping relay to ground the shunt
        }



        /// <summary>
        /// Fills the test dictionary with SRT test entries for the UI
        /// </summary>
        public static void fillTestsDict()
        {
            int count = 0;
            TestUIs.testDictionary = new List<App.TestDictEntry> {
                new App.TestDictEntry(
                    count++,
                    "SRT Hoist Up",
                    "The test checks if the SRT board properly responds to CSW_HOIST_UP CAN communication. " +
                    "When DO15_01 is turned on and CSW_HOIST_UP is sent, AI_01 should read 10V indicating proper hoist up operation.",
                    SRT_Definitions.TestSRTHoistUp(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "SRT Hoist Down",
                    "The test checks if the SRT board properly responds to CSW_HOIST_DOWN CAN communication. " +
                    "When CSW_HOIST_UP is sent, AI_01 should read 10V when DO15_01 is turned on and 0v when DO15_01" +
                    "is off indicating proper hoist down operation.",
                    SRT_Definitions.TestSRTHoistDown(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "REM 1",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestREM1(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "REM 2",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestREM2(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "REMOTE POWER",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestREM_PWR(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "REMOTE FAULT",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestREMOTE_FAULT(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "28V CONTACTOR",
                    "The test checks if the SRT board ",
                    SRT_Definitions.Test28V_CONT(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "AUX CONTACTOR",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestAUX_CONT(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "ESIG",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestESIG(),
                    0, // Empty not testing for Fault
                    false
                ),
                //new App.TestDictEntry(
                //    count++,
                //    "FSIG",
                //    "The test checks if the SRT board ",
                //    SRT_Definitions.TestFSIG(),
                //    0, // Empty not testing for Fault
                //    false
                //),
                new App.TestDictEntry(
                    count++,
                    "FANS HEATSINK",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestFANS_HTSNK(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "FANS TRANSFORMER",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestFANS_XFMR(),
                    0, // Empty not testing for Fault
                    false
                ),
                new App.TestDictEntry(
                    count++,
                    "SHUNT",
                    "The test checks if the SRT board ",
                    SRT_Definitions.TestSHUNT(),
                    0, // Empty not testing for Fault
                    false
                ),
            };
        }
    }
}
