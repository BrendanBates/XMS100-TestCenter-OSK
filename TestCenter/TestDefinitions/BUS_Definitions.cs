﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestCenter.TestProcedures.Procedure;
using static TestCenter.TestDefinitions.DefinitionsCommon;
using static TestCenter.App;
using System.Diagnostics;

namespace TestCenter.TestDefinitions
{
    class BUS_Definitions
    {
        public byte testQuantity = 0;
        //private List<TestPKT> tests_List = new List<TestPKT>();
        public List<Action> busFunc_List = new List<Action>();
        public List<TestPKT> busTests_List = new List<TestPKT>();
        // ANALOG INDEXES
        private const byte IDX_24V_1 = 0; // AN00 
        private const byte IDX_HOIST_HU1 = 1; // AN01 
        private const byte IDX_HOIST_HD1 = 2; // AN02 
        private const byte IDX_SHUNT = 3; // AN03 
        private const byte IDX_24V_2 = 4; // AN04 
        private const byte IDX_FANS_HEAT = 5; // AN05 
        private const byte IDX_FANS_XFMR = 6; // AN06 
        private const byte IDX_28V_CONT = 7; // AN07 
        private const byte IDX_OUT_CONT = 8; // AN08 
        private const byte IDX_ESIG = 9; // AN09 
        private const byte IDX_REM_MON1 = 10; // AN10 
        private const byte IDX_REM_MON2 = 11; // AN11 
        private const byte IDX_REM_ON_LMP = 12; // AN12 
        private const byte IDX_REM_FLT = 13; // AN13 

        public BUS_Definitions()
        {
            fillAllFuncList();
        }

        public List<Action> fillAllFuncList()
        {
            busFunc_List.Clear();          // avoid duplicates

            testQuantity = 0;
            //add all add test functions here
            busFunc_List.Add(clearCAN);
            testQuantity++;
            busFunc_List.Add(OT_Test);
            testQuantity++;
            // busFunc_List.Add(IGBT_1_Test);
            // testQuantity++;
            // busFunc_List.Add(IGBT_2_Test);
            // testQuantity++;
            // busFunc_List.Add(IGBT_3_Test);
            // testQuantity++;
            // busFunc_List.Add(CONT_STS_Test);
            // testQuantity++;
            // busFunc_List.Add(TEMP_SW_Test);
            // testQuantity++;
            // busFunc_List.Add(BUS_UP_Test);
            // testQuantity++;
            // busFunc_List.Add(BUS_VOLT_Test);
            // testQuantity++;
            // busFunc_List.Add(AUTO_MAN_Test);
            // testQuantity++;
            // busFunc_List.Add(CONN_busLK_Test);
            // testQuantity++;
            return busFunc_List;
        }
        public List<TestPKT> reset()
        {
            busTests_List.Clear();
            busTests_List.Add(CAN_CLEAR);
            //SIMULATE WORKING SRT BOARD
            busTests_List.Add(SRT_RESET);
            busTests_List.Add(SRT_400_START);
            //busTests_List.Add(SIM_CSW_GOOD);
            busTests_List.Add(DO_09_ON); // Close 400Hz Contactor
            busTests_List.Add(DO5_01_ON); // Put in Manual Mode
            //TURN OFF IGBT FAULTS
            busTests_List.Add(DO5_02_OFF); // Turn off IGBT A Fault
            busTests_List.Add(DO5_03_OFF); // Turn off IGBT B Fault
            busTests_List.Add(DO5_04_OFF); // Turn off IGBT C Fault
            busTests_List.Add(DO_01_ON); // Turn off IGBT A Overtemp Fault
            busTests_List.Add(DO_02_ON); // Turn off IGBT B Overtemp Fault
            busTests_List.Add(DO_03_ON); // Turn off IGBT C Overtemp Fault
            //TURN OFF OVERTEMP FAULTS
            busTests_List.Add(DO_04_ON); // XFMR Overtemp Fault ON = no fault OFF = fault
            busTests_List.Add(DO_05_ON); // XFMR Overtemp Fault ON = no fault OFF = fault
            busTests_List.Add(DO15_03_ON); // BUS Overtemp Fault ON = no fault OFF = fault
            busTests_List.Add(temp1_OFF);
            busTests_List.Add(temp2_OFF);
            //TURN ON VOLTAGE INPUTS
            AO6_ON.data = new byte[] { 0, 0 };
            busTests_List.Add(AO6_ON);
            AO10_ON.data = new byte[] { 0, 100 };
            busTests_List.Add(AO10_ON);
            AO9_ON.data = new byte[] { 255, 255 };
            busTests_List.Add(AO9_ON);
            AO11_ON.data = new byte[] { 255, 255 };
            busTests_List.Add(AO11_ON);
            busTests_List.Add(SRT_RESET);
            return busTests_List;
        }

        private void clearCAN()
        {
            busTests_List.Clear();
            busTests_List.Add(CAN_CLEAR);
        }

        private void OT_Test()
        {
            busTests_List.Clear();

            //busTests_List.Add(new TestPKT() { type = AnWR, pin = 102, data = new byte[] { 50 } });
            //busTests_List.Add(new TestPKT() { type = CanStart, pin = 109, data = BuildCANData(INTERFACE_BD1, STATUS, new byte[] { 1, 2, 3, 4, 5, 6, 7 })}); // 610 = bus & 250 = Status
            //busTests_List.Add(new TestPKT() { type = AnWR, pin = 66, data = (ushort)75 });
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xB0, data = (ushort)100 });
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xB1, data = (ushort)100 });
            busTests_List.Add(temp1_OFF);
            busTests_List.Add(temp2_OFF);
            busTests_List.Add(DO15_01_ON);
            //busTests_List.Add(CAN_TEST);
            //busTests_List.Add(DO15_02_ON);
            //busTests_List.Add(DO15_03_ON);
            //busTests_List.Add(DO15_04_ON);
            //busTests_List.Add(DO_01_ON);
            //busTests_List.Add(new TestPKT() { type = DoX2, pin = 0xB1, data = new byte[] { 0, 0 } });
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xA6, data = new byte[] { 100, 0 } });

            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void IGBT_1_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(); //8
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void IGBT_2_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xA7, data = new byte[] { 100 } }); //24
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void IGBT_3_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xA6, data = new byte[] { 100 } }); //56
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void CONT_STS_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xA5, data = new byte[] { 100 } }); //120
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void TEMP_SW_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xA4, data = new byte[] { 100 } }); //248
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void BUS_UP_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX1, pin = 0xA3, data = new byte[] { 100 } });
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void BUS_VOLT_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX2, pin = 0xA2, data = new byte[] { 100 } });
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void AUTO_MAN_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX2, pin = 0xA3, data = new byte[] { 100 } });
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }

        private void CONN_INTLK_Test()
        {
            busTests_List.Clear();
            //busTests_List.Add(new TestPKT() { type = DoX2, pin = 0xB2, data = new byte[] { 100 } });
            // Set expected AI values for verification
            App.expectedAI_Vals[IDX_24V_1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HU1] = 0;
            App.expectedAI_Vals[IDX_HOIST_HD1] = 0;
            App.expectedAI_Vals[IDX_SHUNT] = 0;
            App.expectedAI_Vals[IDX_24V_2] = 0;
            App.expectedAI_Vals[IDX_FANS_HEAT] = 0;
            App.expectedAI_Vals[IDX_FANS_XFMR] = 0;
            App.expectedAI_Vals[IDX_28V_CONT] = 0;
            App.expectedAI_Vals[IDX_OUT_CONT] = 0;
            App.expectedAI_Vals[IDX_ESIG] = 0;
            App.expectedAI_Vals[IDX_REM_MON1] = 0;
            App.expectedAI_Vals[IDX_REM_MON2] = 0;
            App.expectedAI_Vals[IDX_REM_ON_LMP] = 0;
            App.expectedAI_Vals[IDX_REM_FLT] = 0;
        }
    }
}