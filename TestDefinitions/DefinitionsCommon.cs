﻿﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestCenter.TestProcedures.Procedure;
using static TestCenter.App;
using System.Windows;
using System.Reflection.Metadata;
using System.ComponentModel;

namespace TestCenter.TestDefinitions
{
    class DefinitionsCommon
    {
        private const byte portA_0 = 0xA0;
        private const byte portA_1 = 0xA1;
        private const byte portA_2 = 0xA2;
        private const byte portA_3 = 0xA3;
        private const byte portA_4 = 0xA4;
        private const byte portA_5 = 0xA5;
        private const byte portA_6 = 0xA6;
        private const byte portA_7 = 0xA7;
        private const byte portB_0 = 0xB0;
        private const byte portB_1 = 0xB1;
        private const byte portB_2 = 0xB2;
        private const byte portB_3 = 0xB3;
        private const byte portB_4 = 0xB4;
        private const byte portB_5 = 0xB5;
        private const byte portB_6 = 0xB6;
        private const byte portB_7 = 0xB7;
        private const byte DO_VADJ1_Pin = 70;
        private const byte DO_VADJ2_Pin = 71;
        private const byte DO_VADJ3_Pin = 68;
        private const byte DO_VADJ4_Pin = 69;
        private const byte DO_VADJ5_Pin = 66;
        private const byte DO_VADJ6_Pin = 67;
        private const byte DO_VADJ7_Pin = 65;
        private const byte DO_VADJ8_Pin = 64;
        private const byte DO_VADJ9_Pin = 99;
        private const byte DO_VADJ10_Pin = 98;
        private const byte DO_VADJ11_Pin = 100;
        private const byte DO_VADJ12_Pin = 101;
        private const byte DO_VADJ13_Pin = 109;
        private const byte DO_VADJ14_Pin = 108;
        private static readonly byte[] DO_OFF = { 0x00, 0x00 };
        private static readonly byte[] TEMP_OFF = { 0xFF, 0xFF };
        private static readonly byte[] DO_ON = { 0xFF, 0xFF };
        private static readonly byte[] TEMP_ON = { 0x00, 0x00 };

        /// STATUS CONSTANTS
        /// SRT status bits for Status1
        private const ushort SRT_S_SYSTEM_FAULT = 1<<0;
        private const ushort SRT_S_400RUN       = 1<<1;
        private const ushort SRT_S_400STOP      = 1<<2;
        private const ushort SRT_S_28RUN        = 1<<3;
        private const ushort SRT_S_28STOP       = 1<<4;
        private const ushort SRT_S_270RUN       = 1<<5;
        private const ushort SRT_S_270STOP      = 1<<6;
        private const ushort SRT_S_E_OUT        = 1<<7;
        private const ushort SRT_S_F_IN         = 1<<8;
        private const ushort SRT_S_OUTCONT      = 1<<9;
        private const ushort SRT_S_FANHTSNK     = 1<<10;
        private const ushort SRT_S_FANXFMR      = 1<<11;
        private const ushort SRT_S_START_SW     = 1<<12;
        private const ushort SRT_S_STOP_SW      = 1<<13;
        private const ushort SRT_S_RESET        = 1<<14;
        private const ushort SRT_S_SYSTEM_OK    = 1<<15;

        // Additional SRT status bits for Status2
        private const ushort SRT_S_MAINT_MODE  = 1<<0;
        private const ushort SRT_S_TESTMODE    = 1<<1;
        private const ushort SRT_S_EF_BYPASS   = 1<<2;
        private const ushort SRT_S_28V_OPTION  = 1<<3;
        private const ushort SRT_S_REAL_OPTION = 1<<4;
        private const ushort SRT_S_REAL_UP     = 1<<5;
        private const ushort SRT_S_REAL_DOWN   = 1<<6;
        private const ushort SRT_S_CONT_AUX    = 1<<7;
        private const ushort SRT_S_DUAL_OUT    = 1<<8;
        private const ushort SRT_S_400RUNAUX   = 1<<9;
        private const ushort SRT_S_400STOPAUX  = 1<<10;

        // CSW status 1 constants
        private const ushort CSW_S_SWITCH1 = 1 << 0;
        private const ushort CSW_S_SWITCH2 = 1 << 1;
        private const ushort CSW_S_SWITCH3 = 1 << 2;
        private const ushort CSW_S_SWITCH4 = 1 << 3;
        private const ushort CSW_S_SWITCH5 = 1 << 4;
        // CSW status 3 constants
        private const ushort CSW_S_400_ON = 1 << 0;
        private const ushort CSW_S_400_OFF = 1 << 1;
        private const ushort CSW_S_OUT_1  = 1 << 2;
        private const ushort CSW_S_OUT_2 = 1 << 3;


        // HOIST UP status constants
        private const ushort CSW_HOIST_UP_Status1 = 0 | CSW_S_SWITCH3;
        private const ushort CSW_HOIST_UP_Status2 = 0;
        private const ushort CSW_HOIST_UP_Status3 = 0 | CSW_S_400_OFF;
        private const ushort CSW_HOIST_UP_Status4 = 100<<8;

        // HOIST DOWN status constants
        private const ushort CSW_HOIST_DOWN_Status1 = 0 | CSW_S_SWITCH4;
        private const ushort CSW_HOIST_DOWN_Status2 = 0;
        private const ushort CSW_HOIST_DOWN_Status3 = 0 | CSW_S_400_OFF;
        private const ushort CSW_HOIST_DOWN_Status4 = 100<<8;

        // SRT_400 status constants: { 210, 147, 0, 4, 0, 0, 0, 100 }       
        private const ushort SRT_400_Status1 = SRT_S_SYSTEM_OK | SRT_S_START_SW | SRT_S_OUTCONT | SRT_S_F_IN | SRT_S_E_OUT | SRT_S_270STOP | SRT_S_28STOP | SRT_S_400RUN;
        private const ushort SRT_400_Status2 = SRT_S_400STOPAUX;
        private const ushort SRT_400_Status3 = 0;
        private const ushort SRT_400_Status4 = 100<<8; // value of last byte must be greater than 0 but less than 250

        // SRT_RESET status constants: { 84, 193, 0, 4, 0, 0, 0, 100 }
        private const ushort SRT_RESET_Status1 = SRT_S_SYSTEM_OK | SRT_S_RESET | SRT_S_270STOP | SRT_S_28STOP | SRT_S_400STOP | SRT_S_F_IN | SRT_S_E_OUT;
        private const ushort SRT_RESET_Status2 = 0;
        private const ushort SRT_RESET_Status3 = 0;
        private const ushort SRT_RESET_Status4 = 100<<8;

        // SRT_28 status constants: { 204, 147, 0, 4, 0, 0, 0, 100 }
        private const ushort SRT_28_Status1 = SRT_S_SYSTEM_OK | SRT_S_START_SW | SRT_S_OUTCONT | SRT_S_F_IN | SRT_S_E_OUT | SRT_S_270STOP | SRT_S_28RUN;
        private const ushort SRT_28_Status2 = SRT_S_TESTMODE | SRT_S_400STOPAUX;
        private const ushort SRT_28_Status3 = 0;
        private const ushort SRT_28_Status4 = 100<<8;

        // SRT_EF_FAIL status constants: { 210, 146, 0, 4, 0, 0, 0, 100 }
        private const ushort SRT_EF_FAIL_Status1 = SRT_S_SYSTEM_OK | SRT_S_START_SW | SRT_S_OUTCONT | SRT_S_E_OUT | SRT_S_270STOP | SRT_S_28STOP | SRT_S_400RUN;
        private const ushort SRT_EF_FAIL_Status2 = SRT_S_400STOPAUX;
        private const ushort SRT_EF_FAIL_Status3 = 0;
        private const ushort SRT_EF_FAIL_Status4 = 100<<8;

        // SRT_GOOD_TESTMODE status constants: { 18, 129, 2, 4, 0, 0, 0, 100 }
        private const ushort SRT_TESTMODE_Status1 = SRT_S_START_SW | SRT_S_400RUN;
        private const ushort SRT_TESTMODE_Status2 = SRT_S_MAINT_MODE | SRT_S_TESTMODE;
        private const ushort SRT_TESTMODE_Status3 = SRT_S_EF_BYPASS;
        private const ushort SRT_TESTMODE_Status4 = 100<<8;


        private const ushort Status4 = 100 << 8;

        /// TEST TYPES - 
        private const byte AnalogStart = 0x00;
        private const byte AnalogStop = 0x01;
        private const byte DoX1 = 0x10; // Digital Output for Expander 1 = EXP40
        private const byte DoX2 = 0x11; // Digital Output for Expander 2 = EXP42
        private const byte CanStart = 0x20; // CAN Write Start Command
        private const byte CanStop = 0x21; // CAN Write Stop Command
        private const byte CanSingle = 0x22; // CAN Write Stop Command
        private const byte CanClear = 0x30; // Stop All CAN Commands
        private const byte tempWR = 0x30;
        private const byte tempRD = 0x31;
        private const byte pwmWR = 0x40;
        private const byte pwmRD = 0x41;




        /// !!! DIGITALS !!! -----------------------------------------------------------------------------------------------------
        /// DIGITAL OUTPUTS OFF:
        /// 5 VOLTS -
        public readonly static TestPKT DO5_01_OFF = new TestPKT() { type = DoX2, pin = portB_3, data = DO_OFF }; // Turn off 5v digital output 1
        public readonly static TestPKT DO5_02_OFF = new TestPKT() { type = DoX2, pin = portB_2, data = DO_OFF }; // Turn off 5v digital output 2
        public readonly static TestPKT DO5_03_OFF = new TestPKT() { type = DoX2, pin = portB_5, data = DO_OFF }; // Turn off 5v digital output 3
        public readonly static TestPKT DO5_04_OFF = new TestPKT() { type = DoX2, pin = portB_4, data = DO_OFF }; // Turn off 5v digital output 4
        /// 15 VOLTS -
        public readonly static TestPKT DO15_01_OFF = new TestPKT() { type = DoX2, pin = portB_1, data = DO_OFF }; // Turn off 15v digital output 1
        public readonly static TestPKT DO15_02_OFF = new TestPKT() { type = DoX2, pin = portB_0, data = DO_OFF }; // Turn off 15v digital output 2
        public readonly static TestPKT DO15_03_OFF = new TestPKT() { type = DoX2, pin = portA_7, data = DO_OFF }; // Turn off 15v digital output 3
        public readonly static TestPKT DO15_04_OFF = new TestPKT() { type = DoX2, pin = portA_6, data = DO_OFF }; // Turn off 15v digital output 4
        /// 24 VOLTS -
        public readonly static TestPKT DO24_01_OFF = new TestPKT() { type = DoX2, pin = portA_5, data = DO_OFF }; // Turn off 24v digital output 1
        public readonly static TestPKT DO24_02_OFF = new TestPKT() { type = DoX2, pin = portA_4, data = DO_OFF }; // Turn off 24v digital output 2
        public readonly static TestPKT DO24_03_OFF = new TestPKT() { type = DoX2, pin = portA_3, data = DO_OFF }; // Turn off 24v digital output 3
        public readonly static TestPKT DO24_04_OFF = new TestPKT() { type = DoX2, pin = portA_2, data = DO_OFF }; // Turn off 24v digital output 4
        /// TEMPERATURE -
        public readonly static TestPKT temp1_OFF = new TestPKT() { type = DoX2, pin = portB_6, data = TEMP_OFF }; // Turn off relay for the digital temperature 1 active low
        public readonly static TestPKT temp2_OFF = new TestPKT() { type = DoX2, pin = portB_7, data = TEMP_OFF }; // Turn off relay for the digital temperature 2 active low
        /// DRY CONTACTS -
        public readonly static TestPKT DO_01_OFF = new TestPKT() { type = DoX1, pin = portA_7, data = DO_OFF }; // Turn off dry contact relay 1
        public readonly static TestPKT DO_02_OFF = new TestPKT() { type = DoX1, pin = portA_6, data = DO_OFF }; // Turn off dry contact relay 2
        public readonly static TestPKT DO_03_OFF = new TestPKT() { type = DoX1, pin = portA_5, data = DO_OFF }; // Turn off dry contact relay 3
        public readonly static TestPKT DO_04_OFF = new TestPKT() { type = DoX1, pin = portA_4, data = DO_OFF }; // Turn off dry contact relay 4
        public readonly static TestPKT DO_05_OFF = new TestPKT() { type = DoX1, pin = portA_3, data = DO_OFF }; // Turn off dry contact relay 5
        public readonly static TestPKT DO_06_OFF = new TestPKT() { type = DoX1, pin = portB_4, data = DO_OFF }; // Turn off dry contact relay 6
        public readonly static TestPKT DO_07_OFF = new TestPKT() { type = DoX1, pin = portB_3, data = DO_OFF }; // Turn off dry contact relay 7
        public readonly static TestPKT DO_08_OFF = new TestPKT() { type = DoX1, pin = portB_2, data = DO_OFF }; // Turn off dry contact relay 8
        public readonly static TestPKT DO_09_OFF = new TestPKT() { type = DoX1, pin = portB_1, data = DO_OFF }; // Turn off dry contact relay 9
        public readonly static TestPKT DO_10_OFF = new TestPKT() { type = DoX1, pin = portB_0, data = DO_OFF }; // Turn off dry contact relay 10


        /// DIGITAL OUTPUTS ON:
        /// 5 VOLTS -
        public readonly static  TestPKT DO5_01_ON = new TestPKT() { type = DoX2, pin = portB_3, data = DO_ON }; // Turn on 5v digital output 1
        public readonly static  TestPKT DO5_02_ON = new TestPKT() { type = DoX2, pin = portB_2, data = DO_ON }; // Turn on 5v digital output 2
        public readonly static  TestPKT DO5_03_ON = new TestPKT() { type = DoX2, pin = portB_5, data = DO_ON }; // Turn on 5v digital output 3
        public readonly static  TestPKT DO5_04_ON = new TestPKT() { type = DoX2, pin = portB_4, data = DO_ON }; // Turn on 5v digital output 4
        /// 15 VOLTS -
        public readonly static  TestPKT DO15_01_ON = new TestPKT() { type = DoX2, pin = portB_1, data = DO_ON }; // Turn on 15v digital output 1
        public readonly static  TestPKT DO15_02_ON = new TestPKT() { type = DoX2, pin = portB_0, data = DO_ON }; // Turn on 15v digital output 2
        public readonly static  TestPKT DO15_03_ON = new TestPKT() { type = DoX2, pin = portA_7, data = DO_ON }; // Turn on 15v digital output 3
        public readonly static  TestPKT DO15_04_ON = new TestPKT() { type = DoX2, pin = portA_6, data = DO_ON }; // Turn on 15v digital output 4
        /// 24 VOLTS -
        public readonly static  TestPKT DO24_01_ON = new TestPKT() { type = DoX2, pin = portA_5, data = DO_ON }; // Turn on 24v digital output 1
        public readonly static  TestPKT DO24_02_ON = new TestPKT() { type = DoX2, pin = portA_4, data = DO_ON }; // Turn on 24v digital output 2
        public readonly static  TestPKT DO24_03_ON = new TestPKT() { type = DoX2, pin = portA_3, data = DO_ON }; // Turn on 24v digital output 3
        public readonly static  TestPKT DO24_04_ON = new TestPKT() { type = DoX2, pin = portA_2, data = DO_ON }; // Turn on 24v digital output 4
        /// TEMPERATURE -
        public readonly static  TestPKT temp1_ON = new TestPKT() { type = DoX2, pin = portB_6, data = TEMP_ON }; // Turn on relay for the digital temperature 1 active low
        public readonly static  TestPKT temp2_ON = new TestPKT() { type = DoX2, pin = portB_7, data = TEMP_ON }; // Turn on relay for the digital temperature 2 active low
        /// DRY CONTACTS -
        public readonly static  TestPKT DO_01_ON = new TestPKT() { type = DoX1, pin = portA_7, data = DO_ON }; // Turn on dry contact relay 1
        public readonly static  TestPKT DO_02_ON = new TestPKT() { type = DoX1, pin = portA_6, data = DO_ON }; // Turn on dry contact relay 2
        public readonly static  TestPKT DO_03_ON = new TestPKT() { type = DoX1, pin = portA_5, data = DO_ON }; // Turn on dry contact relay 3
        public readonly static  TestPKT DO_04_ON = new TestPKT() { type = DoX1, pin = portA_4, data = DO_ON }; // Turn on dry contact relay 4
        public readonly static  TestPKT DO_05_ON = new TestPKT() { type = DoX1, pin = portA_3, data = DO_ON }; // Turn on dry contact relay 5
        public readonly static  TestPKT DO_06_ON = new TestPKT() { type = DoX1, pin = portB_4, data = DO_ON }; // Turn on dry contact relay 6
        public readonly static  TestPKT DO_07_ON = new TestPKT() { type = DoX1, pin = portB_3, data = DO_ON }; // Turn on dry contact relay 7
        public readonly static  TestPKT DO_08_ON = new TestPKT() { type = DoX1, pin = portB_2, data = DO_ON }; // Turn on dry contact relay 8
        public readonly static  TestPKT DO_09_ON = new TestPKT() { type = DoX1, pin = portB_1, data = DO_ON }; // Turn on dry contact relay 9
        public readonly static  TestPKT DO_10_ON = new TestPKT() { type = DoX1, pin = portB_0, data = DO_ON }; // Turn on dry contact relay 10



        /// !!! ANALOG OUTPUTS !!! ------------------------------------------------------------------------------------------------
        /// STOP ANALOG OUTPUTS:
        public readonly static TestPKT AO1_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ1_Pin, data = DO_OFF }; // Turn off analog output 1
        public readonly static TestPKT AO2_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ2_Pin, data = DO_OFF }; // Turn off analog output 2
        public readonly static TestPKT AO3_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ3_Pin, data = DO_OFF }; // Turn off analog output 3
        public readonly static TestPKT AO4_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ4_Pin, data = DO_OFF }; // Turn off analog output 4
        public readonly static TestPKT AO5_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ5_Pin, data = DO_OFF }; // Turn off analog output 5
        public readonly static TestPKT AO6_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ6_Pin, data = DO_OFF }; // Turn off analog output 6
        public readonly static TestPKT AO7_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ7_Pin, data = DO_OFF }; // Turn off analog output 7
        public readonly static TestPKT AO8_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ8_Pin, data = DO_OFF }; // Turn off analog output 8
        public readonly static TestPKT AO9_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ9_Pin, data = DO_OFF }; // Turn off analog output 9
        public readonly static TestPKT AO10_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ10_Pin, data = DO_OFF }; // Turn off analog output 10
        public readonly static TestPKT AO11_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ11_Pin, data = DO_OFF }; // Turn off analog output 11
        public readonly static TestPKT AO12_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ12_Pin, data = DO_OFF }; // Turn off analog output 12
        public readonly static TestPKT AO13_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ13_Pin, data = DO_OFF }; // Turn off analog output 13
        public readonly static TestPKT AO14_OFF = new TestPKT() { type = AnalogStop, pin = DO_VADJ14_Pin, data = DO_OFF }; // Turn off analog output 14


        /// START ANALOG OUTPUTS:
        public static TestPKT AO1_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ1_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 1
        public static TestPKT AO2_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ2_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 2
        public static TestPKT AO3_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ3_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 3
        public static TestPKT AO4_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ4_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 4
        public static TestPKT AO5_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ5_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 5
        public static TestPKT AO6_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ6_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 6
        public static TestPKT AO7_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ7_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 7
        public static TestPKT AO8_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ8_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 8
        public static TestPKT AO9_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ9_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 9
        public static TestPKT AO10_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ10_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 10
        public static TestPKT AO11_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ11_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 11
        public static TestPKT AO12_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ12_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 12
        public static TestPKT AO13_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ13_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 13
        public static TestPKT AO14_ON = new TestPKT() { type = AnalogStart, pin = DO_VADJ14_Pin, data = new byte[] { 0, 64 } }; // Turn on analog output 14        /// !!! CAN COMMUNICATIONS !!! ------------------------------------------------------------------------------------------
        /// CAN TX COMMANDS:
        public readonly static TestPKT SRT_RESET = new TestPKT() {
            type = CanSingle,
            pin = 20,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_RESET_Status1, SRT_RESET_Status2, SRT_RESET_Status3, SRT_RESET_Status4))
        };
        public readonly static TestPKT SRT_RESET_STOP = new TestPKT() {
            type = CanStop,
            pin = 109,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_RESET_Status1, SRT_RESET_Status2, SRT_RESET_Status3, SRT_RESET_Status4))
        };
        public readonly static TestPKT SRT_28_START = new TestPKT() {
            type = CanStart,
            pin = 50,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_28_Status1, SRT_28_Status2, SRT_28_Status3, SRT_28_Status4))
        };
        public readonly static TestPKT SRT_28_STOP = new TestPKT() {
            type = CanStop,
            pin = 50,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_28_Status1, SRT_28_Status2, SRT_28_Status3, SRT_28_Status4))
        };
        public readonly static TestPKT SRT_400_START = new TestPKT() {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_400_Status1, SRT_400_Status2, SRT_400_Status3, SRT_400_Status4))
        }; // 620 = SRT_Status & 210 = 400hz on 28v off & 270v off Esig on
        public readonly static TestPKT SRT_400_STOP = new TestPKT() {
            type = CanStop,
            pin = 109,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_400_Status1, SRT_400_Status2, SRT_400_Status3, SRT_400_Status4))
        }; // 620 = SRT_Status & 210 = 400hz on 28v off & 270v off Esig on
        public readonly static TestPKT SRT_EF_FAIL = new TestPKT() {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_EF_FAIL_Status1, SRT_EF_FAIL_Status2, SRT_EF_FAIL_Status3, SRT_EF_FAIL_Status4))
        }; // 620 = SRT_Status & 210 = 400hz on 28v off & 270v off Esig on
        public readonly static TestPKT SRT_GOOD_TESTMODE = new TestPKT() {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    SRT_S_BD1,
                    STATUS,
                    StatusIntsToBytes(SRT_TESTMODE_Status1, SRT_TESTMODE_Status2, SRT_TESTMODE_Status3, SRT_TESTMODE_Status4))
        }; // 620 = SRT_Status & 210 = 400hz on 28v off & 270v off Esig on
        public readonly static TestPKT CAN_CLEAR = new TestPKT() {
            type = CanClear,
            pin = 0,
            data = new byte[] { 0, 0 } 
        };
        public readonly static TestPKT CSW_HOIST_UP = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    CSW1_S_1,
                    STATUS,
                    StatusIntsToBytes(CSW_HOIST_UP_Status1, CSW_HOIST_UP_Status2, CSW_HOIST_UP_Status3, CSW_HOIST_UP_Status4))
        }; 
        public readonly static TestPKT CSW_HOIST_DOWN = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    CSW1_S_1,
                    STATUS,
                    StatusIntsToBytes(CSW_HOIST_DOWN_Status1, CSW_HOIST_DOWN_Status2, CSW_HOIST_DOWN_Status3, CSW_HOIST_DOWN_Status4))
        };
        public readonly static TestPKT CSW_400_ON = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    CSW1_S_1,
                    STATUS,
                    StatusIntsToBytes(0, 0, 1, Status4))
        };
        public readonly static TestPKT CSW_400_OFF = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    CSW1_S_1,
                    STATUS,
                    StatusIntsToBytes(0, 0, 2, Status4))
        };
        public readonly static TestPKT CSW_28V_ON = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    CSW1_S_1,
                    STATUS,
                    StatusIntsToBytes(0, 0, 6, Status4))
        };
        public readonly static TestPKT CSW_AUXCONT_ON = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    CSW1_S_1,
                    STATUS,
                    StatusIntsToBytes(0, 0, 10, Status4))
        };
        public readonly static TestPKT CSW_CONT_OFF = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    CSW1_S_1,
                    STATUS,
                    StatusIntsToBytes(2, 0, 0, Status4))
        };
        public readonly static TestPKT INT_400_ON = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    INTERFACE_BD1,
                    STATUS,
                    StatusIntsToBytes(0, 2, 0, Status4))
        };
        public readonly static TestPKT INT_28V_ON = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    INTERFACE_BD1,
                    STATUS,
                    StatusIntsToBytes(0, 1, 0, Status4))
        };
        public readonly static TestPKT INT_ALL_OFF = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    INTERFACE_BD1,
                    STATUS,
                    StatusIntsToBytes(0, 512, 0, Status4))
        };
        public readonly static TestPKT INT_S_FAULT = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    INTERFACE_BD1,
                    STATUS,
                    StatusIntsToBytes(1, 0, 0, Status4))
        };
        public readonly static TestPKT INT_AUX1_OT = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    INTERFACE_BD1,
                    FAULT,
                    StatusIntsToBytes(0, 0, 32, Status4))
        };
        public readonly static TestPKT INT_S_XFMR_T1 = new TestPKT()
        {
            type = CanStart,
            pin = 109,
            data = BuildCANData(
                    INTERFACE_BD1,
                    STATUS,
                    StatusIntsToBytes(0, 1<<13, 0, Status4))
        };



        public static byte[] BuildCANData(long id, byte status, byte[] data)
         {
            if (data.Length > 8)
            {
                throw new ArgumentException("Data length cannot exceed 8 bytes.");
            }
            
             byte[] canData = new byte[12]; // 4 bytes for ID, 7 bytes for data, and 1 byte for status
             canData[0] = (byte)(id >> 24); // Shift ID to get the first byte
             canData[1] = (byte)(id >> 16); // Shift ID to get the second byte
             canData[2] = (byte)(id >> 8);  // Shift ID to get the third byte
             canData[3] = (byte)(id & 0xFF); // Get the last byte of ID
             Array.Copy(data, 0, canData, 4, data.Length); // Copy data into the array starting from index 4 
            // Pad remaining data bytes with zeros if data.Length < 7
            for (int i = 4 + data.Length; i < 11; i++)
            {
                canData[i] = 0;
            }
             canData[11] = status; // Set the status byte at the end of the array
             // Status byte is the last data byte in the array
            // Data array is automatically zero-initialized, no need for explicit padding
            return canData;
         }
    }
}
