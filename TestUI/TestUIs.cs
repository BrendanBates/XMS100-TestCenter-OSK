using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TestCenter.TestDefinitions;

namespace TestCenter.TestUI
{
    public class TestUIs 
    {        

        public static List<App.TestDictEntry> testDictionary = new List<App.TestDictEntry>();


        public static void InitTestDisplay(uint _deviceID)
        {
            testDictionary.Clear();
            //App.deviceID = _deviceID;
            switch (_deviceID)
            {
                case 0: IntDisplay(); break;
                case 1: HzDisplay(); break;
                case 2: SrtDisplay(); break;
                case 3: BusDisplay(); break;
                default:
                    //ERROR NO SUCH DISPLAY 
                    break;
            }
        }

        private static void IntDisplay(/*Maybe pass in an array of selected tests*/ )
        {
            INT_Definitions.fillTestDict();
        }

        private static void SrtDisplay(/*Maybe pass in an array of selected tests*/ )
        {
            SRT_Definitions.fillTestsDict();
        }
        
        private static void BusDisplay(/*Maybe pass in an array of selected tests*/ )
        {
            // BUS test dictionary will be populated here when BUS tests are implemented
            // Currently no active BUS tests defined
        }
        
        private static void HzDisplay(/*Maybe pass in an array of selected tests*/ )
        {
            // Hz test dictionary will be populated here when Hz tests are implemented
            // Currently no active Hz tests defined
        }
    }
}
