﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using HidSharp;
using static TestCenter.App;

namespace TestCenter.TestDefinitions
{
    class Definitions
    {
        public List<TestPKT> tests_List = new List<TestPKT>();
        public List<TestPKT> resetList = new List<TestPKT>();

        public Definitions()
        {
            //fillAllFuncList(deviceID);
        }

        private void fillAllFuncList(uint _deviceID)
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

        
        private void fillINTFuncList()
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

        private void fillSRTFuncList()
       {
            var srtDefinitions = new SRT_Definitions(); // Create an instance of SRT_Definitions  
            resetList = srtDefinitions.reset();
            tests_List = SRT_Definitions.srtTests_List;
            StatusID_1 = SRT_S_BD1;
            StatusID_2 = SRT_S_BD2;
        }

        private void fillHzFuncList()
        {
            var HzDefinitions = new HZ_Definitions(); // Create an instance of HZ_Definitions  
            resetList = HzDefinitions.reset();
            tests_List = HzDefinitions.hzTests_List;
        }

        private void fillBUSFuncList()
        {
            var busDefinitions = new BUS_Definitions(); // Create an instance of BUS_Definitions  
            resetList = busDefinitions.reset();
            tests_List = busDefinitions.busTests_List;
        }

    }
}

