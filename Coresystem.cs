using Sandbox;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRageMath;
using CoreOs;
using System.Collections;

namespace core
{
    public class mainframe
    {
        IMyGridTerminalSystem GridTerminalSystem; // comment me out in space engineers
        bool VSTUDIO = false;

        /// RECOMMERED:
        /// 
        // To use an EXTERNAL CONFIG SCREEN you have to rename one LCD on your GRID with the following ID 
        // (and only with this ID!)
        const string configID = "_SIOS_Config_";
        // To use a Debugger Screen you have to set the desired Screens PUBLIC TITLE to the following ID:
        const string debugID = "_SIOS_CORE_Debug_";
        ///<SUMMARY>
        // Mandatory Blockname of your Mainframe
        const string coreID = "_SIOS_CORE_MAINFRAME_";
        // Mandatory Blockname of your EDI Defence Mainframe
        const string EDIID = "_SIOS_EDI_ADDON_";

        Dictionary<string, string> configDict;

        const string creator = "Mahtrok";
        const string cocreator = "cccdemon";
        const string title = "$IOS <CORE> ";
        const string version = "v1.1";
        const string created = "10-09-15";
        const string updated = "10-24-15";
        const string contact = "Mahtrok@mahtrok.de";
        const string company = "Exodus Systems - 2015";
        /// Exodus Systems - $IOS - Self installing operating system <CORE>
        /// As the name mentions this Script is used to install itself into an existing Grid (Small-, Large Ship or Station) by
        /// renaming all Blocks. The Script adds a platformID and different block IDs into every block to be compatible 
        /// with all Exodus Systems Scripts.
        /// It also provides the abillity to be configurated by an external config LCD (has to be named as "$IOS Config") and
        /// will be filled with default values if not manually set up. 
        /// This single config screen will be used by each of the Mahriane Soft Scripts you install into your Grid, you can 
        /// then configure them all from this screen.
        /// Once installed all Mahriane Soft Scripts will be able to identify the Blocks they require by searching for the 
        /// exact same blockID's. By providing a platformID inside the config Screen, all Mahriane Soft Scripts will use 
        /// only those blocks belonging to the parent Platform / Ship.
        /// - In case you want to use the Script without an external configuration screen, you will have to add this platform
        /// ID manually to all Scripts and configure them one by one.
        /// - In case you leave the platform ID empty, each Script will create a random platform ID when it is running the 
        /// first time which will cause problems if every Script uses its own platformID. So ensure to add 1 unique ID to all
        /// Scripts manually!
        /// The Script also provides some argument management (you can find all available arguments @ the end of this 
        /// summary.)
        /*
        EXTERNAL CONFIG STRUCTURE: (If Config screen is left empty it will automatically be written the first run if named correct)
        $platformID: 
        $platformRoleID: 0
        $condition: green
        $install enabled: False
        $hide version info: False
        $allow beacon rename: False
        $activate security: True
        $LCD BG Color: {R:40 G:40 B:40 A:255}
        $LCD Font Color: {R:0 G:0 B:0 A:255}
        AVAILABLE PASS-IN ARGUMENTS:
        Delete: text                             // Deletes the passed in "text" within all blocks custom names
        Replace: text1, text2               // Replaces the passed in "text1" with "text2" within all custom names
        Install                                      // sets installEnabled == true for 1 single run of the Script.
        Uninstall                                  // removes all $IOS IDs within all blocks custom names
                                                        // ATTENTION: To uninstall you have to use the correct (if existing) platformID!
        new ID                                     // generates a new random ID (0-1000), stores this new ID @ config Screen 
                                                        // if external config is used.
        contact                                    // Debugs the Contact data
        condition: red, orange, green // Changing the condition, used to change colors on LCD screens and interior Lights
        reboot                                      // Reinitializing the Script
        ATTENTION: Except the Config and the Debug Screen, all LCDs that should be used by $IOS have to be public titled
        with the infoID! "$Info" by default
        LCDs that are titled something else, will not be used at any time, so you will not get a visual output as well!
        */
        ///</SUMMARY>
        // << 1.0 GENERAL SETTINGS >> //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // NOTE: If you are using an external config screen LEAVE THESE VALUES AS THEY ARE! They will be overwritten.   //
        // If you are configuring this Script manually follow the instructions. Read carefully!                                                     //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // The platform ID. SHOULD be a short number between 0 and 1000, but CAN BE anything.
        string platformID = "";
        // Manually set script to installation mode. Program will rename all functional blocks not yet having the platformID.
        // NOTE: If any ships are docked with the platform while install is running, their blocks will be renamed too! So ensure
        // to undock everyone before you activate install!
        bool installEnabled = false;
        // Display the script version info on all LCD screens? If not (false), it will still be shown on the debugger screen!
        bool hideVersionInfoOnScreen = false;
        // Should this Script be allowed to automatically rename the Beacons on this Grid?
        bool allowBeaconRename = false;
        // << 2.0 BLOCK ID SETTINGS >> //
        // These Shortcuts will be added to the blocks CustomNames to be identified by all Mahriane Soft Scripts. Not every Script
        // will need all IDs. If you want to change these IDs Uninstall the program first, then change the IDs and reinstall the Script.
        // Ensure that you add your adjusted IDs to all Mahriane Soft Scripts so they are able to find their blocks!
        const string assemblerID = "$Asse";
        const string refineryID = "$Refi";
        const string cargoID = "$Carg";
        const string connectorID = "$Conn";
        const string collectorID = "$Coll";
        const string batteryID = "$Batt";
        const string gravityID = "$Grav";
        const string reactorID = "$Reac";
        const string sorterID = "$Sort";
        const string oxygenID = "$OxyG";
        const string oxyFarmID = "$OxyF";
        const string oxyTankID = "$OxyT";
        const string airVentID = "$Vent";
        const string doorID = "$Door";
        const string storageID = "$Stor";
        const string productionID = "$Prod";
        const string speakerID = "$Spea";
        const string lightID = "$Light";
        const string terminalID = "$Ctrl";
        const string sensorID = "$Sen";
        const string panelID = "$LCD";
        const string infoID = "$Info";
        const string programID = "$Prog";
        const string timerID = "$Time";
        const string missileID = "$MTur";
        const string gatlingID = "$GTur";
        const string turretID = "$ITur";
        const string thrusterID = "$Thru";
        // if new block and not yet observed by this script
        const string noConfigID = "$NA";
        const string traderID = "$IOS <TRADER>";
        const string updateID = "$IOS <UPDATE>";
        // << 3.0 COLOR SETTINGS >> //
        // Condition Manager: To define the background and font Color for your LCDs
        Color conditionRedBG = new Color(40, 0, 0);
        Color conditionOrangeBG = new Color(40, 10, 0);
        Color panelDefaultBG = new Color(40, 40, 40);
        Color conditionRedFC = new Color(255, 255, 255);
        Color conditionOrangeFC = new Color(255, 255, 255);
        Color panelDefaultFC = new Color(0, 0, 0);
        // Condition Manager: To define the Colors for the Interior Lights
        Color conditionRedLight = new Color(255, 0, 0);
        Color conditionOrangeLight = new Color(255, 110, 0);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // THERE IS NO NEED TO CHANGE ANYTHING BELOW THIS LINE, AS LONG AS YOU DO NOT EXACTLY KNOW WHAT  //
        // YOU ARE DOING!                                                                                                                                                              //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        const char divider = '|';
        const char DIVIDER = '|';

        const char listDivider = ',';
        const char configDivider = '$';
        const char configSeparator = ':';

        const char suffix = ']';
        const char SUFFIX = ']';
        const string prefix = "[";
        const string PREFIX = "[";
        bool debugEnabled = false;
        bool booted = false;
        bool securityActive = false;
        bool initialized = false;
        bool enabled = true;

        /// <Command API>
        const string API_RAISE_ALARM = "API_RAISE_ALARM";
        const string API_DISABLE_ALARM = "API_DISABLE_ALARM";
        const string API_SHUTDOWN_SHIP = "API_SHUTDOWN_SHIP";
        const string API_BOOT_SHIP = "API_BOOT_SHIP";
        const string API_DISABLE_BLOCK = "API_DISABLE_BLOCK";
        const string API_ENABLE_BLOCK = "API_ENABLE_BLOCK";
        const string API_ATTACK_DETECTED = "API_ATTACK_DETECTED";
        const string API_ATTACK_DEFENDET = "API_ATTACK_DEFENDET";
        const string API_COLLISION_ALERT = "API_ATTACK_DEFENDET";
        const string API_STATUS_GREEN = "API_STATUS_GREEN";
        const string API_STATUS_ORANGE = "API_STATUS_ORANGE";
        const string API_STATUS_RED = "API_STATUS_RED";
        const string API_CANT_PRESURISE = "API_CANT_PRESURISE";

        const string API_EDI_LOGON = "API_EDI_LOGON";
        const string API_EDI_LOGOFF = "API_EDI_LOGOFF";

        const string API_TVI_LOGON = "API_TVI_LOGON";
        const string API_TVI_LOGOFF = "API_TVI_LOGOFF";

        const string API_COM_LOGON = "API_COM_LOGON";
        const string API_COM_LOGOFF = "API_COM_LOGOFF ";
        /// </Command API>

        /// Config Text Consts
        const string KEY_PLATTFORM_ID = "C_PLATTFORM_ID";
        const string KEY_PLATFORM_ROLE_ID = "C_PLATFORM_ROLE_ID";
        const string KEY_CONDITION = "C_CONDITION";
        const string KEY_INSTALL_ENABLED = "C_INSTALL_ENABLED";
        const string KEY_HIDE_VERSION_INFO = "C_HIDE_VERSION_INFO";
        const string KEY_ALLOW_BEACON_RENAME = "C_ALLOW_BEACON_RENAME";
        const string KEY_ACTIVATE_SECURITY = "C_ACTIVATE_SECURITY";
        const string KEY_LCD_BG_COLOR = "C_LCD_BG_COLOR";
        const string KEY_LCD_FONT_COLOR = "C_LCD_FONT_COLOR";
        const string KEY_DEBUG_ENABLED = "C_DEBUG_ENABLED";
        const string KEY_EDI_INSTALLED = "C_EDI_INSTALLED";
        const string KEY_ATI_INSTALLED = "C_ATI_INSTALLED";

        
        string VALUE_PLATTFORM_ID = "0";
        string VALUE_PLATFORM_ROLE_ID = "0";
        string VALUE_CONDITION = "greed";
        string VALUE_INSTALL_ENABLED = "false";
        string VALUE_HIDE_VERSION_INFO = "true";
        string VALUE_ALLOW_BEACON_RENAME = "false";
        string VALUE_ACTIVATE_SECURITY = "false";
        string VALUE_LCD_BG_COLOR = "40,40,40";
        string VALUE_LCD_FONT_COLOR = "255,255,255";
        string VALUE_DEBUG_ENABLED = "false";
        string VALUE_EDI_INSTALLED = "false";
        string VALUE_ATI_INSTALLED = "false";




        List<string> debugMessages;
        List<IMyAssembler> assemblers;
        List<IMyRefinery> refineries;
        List<IMyConveyorSorter> sorters;
        List<IMyOxygenGenerator> oxygens;
        List<IMyOxygenTank> oxytanks;
        List<IMyAirVent> airvents;
        List<IMyBatteryBlock> batteries;
        List<IMyReactor> reactors;
        List<IMyInventory> storage;
        List<IMyInventory> production;
        List<IMySoundBlock> speakers;
        List<IMyInteriorLight> lights;
        List<IMyDoor> doors;
        List<IMySensorBlock> sensors;
        List<IMyTerminalBlock> terminals;
        List<IMyTextPanel> panels;
        List<IMyTextPanel> debugger;
        IMyTextPanel config;
        List<IMyProgrammableBlock> programs;
        List<IMyProgrammableBlock> traders;
        List<IMyTimerBlock> timers;
        //IMyTimerBlock securityTimer;
        IMyProgrammableBlock core;
        IMyProgrammableBlock EDI;
        List<IMyLargeGatlingTurret> gatlings;
        List<IMyLargeMissileTurret> missiles;
        List<IMyLargeInteriorTurret> turrets;
        List<MARole> platformRoles;
        int platformRoleID = 0;
        int airVentCnt = 0;
        const string blank = "\n";
        const string tab = "    ";
        const string empty = " ";
        string condition = "green";


        bool ediInstalled = false;
        bool tmviInstalled = false;


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                            GETTER                                                                                  */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        int GetRandomPlatformID()
        {
            Random _rnd = new Random();
            return _rnd.Next(1000);
        }
        string GetTimeStamp()
        {
            return prefix + System.DateTime.Now.Hour.ToString() + ":"
            + System.DateTime.Now.Minute.ToString() + ":"
            + System.DateTime.Now.Second.ToString() + suffix;
        }
        string GetVersionInfo()
        {
            return title + empty + version + " by " + creator + ", last Update: " + updated + blank;
        }
        string GetContactData()
        {
            return blank + "Creator: " + creator + blank + "E-Mail: " + contact + blank + company;
        }
        string GetCoContactData()
        {
            return blank + "CoAutor: " + cocreator + blank;
        }
        string GetDebugInfo()
        {
            string _info = GetVersionInfo();
            for (int i = 0; i < debugMessages.Count; i++)
            {
                _info += debugMessages[i];
            }
            return _info;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                       MAIN METHOD                                                                           */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void start(string argument)
        {
            Main(argument);
        }
        void Main(string argument)
        {

            if (argument == "reinstall") {
               initialized = false;
            }
                
            ///////////////////// INITIALIZATION /////////////////////
            if (!initialized) {
                _generateConfig();
                Init();
                Debug("System Initialized");
            }
            ////////////////////// DEBUGGER ///////////////////////
            if (debugEnabled)
                DisplayDebug();
            if (argument == "reload")
            {
                LoadExternalConfigData();
            }


            /*if (enabled)
            {
                if (argument != "")
                    ProcessArgument(argument);
                ///////////////////// INSTALLATION /////////////////////
                if (installEnabled)
                    Install();
                ////////////////// GETTING ALL BLOCKs /////////////////
                if (!booted && initialized)
                {
                    GetBlocks();
                }
            }
            StoreExternalConfigData();
            /////////////////////// DISPLAY /////////////////////////
            Display();
            ///////////////////// RESET SCRIPT ////////////////////
            Reset();*/
        }
        void ProcessArgument(string _argument)
        {
            _argument = _argument.ToLower();
            if (_argument.StartsWith("delete: "))
            {
                _argument = _argument.Substring(8);
                Delete(_argument);
            }
            else if (_argument.StartsWith("replace: "))
            {
                _argument = _argument.Substring(9);
                Replace(_argument);
            }
            else if (_argument.StartsWith("condition: "))
            {
                _argument = _argument.Substring(11).ToLower();
                SetCondition(_argument.Replace(empty, "").Replace(blank, ""));
            }
            else if (_argument.StartsWith("api: "))
            {
                Debug("Argument Starts with API - Another Block calls an action");
                ProcessAPIArgument(_argument);
            }

            else
            {
                Debug("Argument: " + _argument);
                switch (_argument)
                {
                    case "new id": platformID = GetRandomPlatformID().ToString(); StoreExternalConfigData(); break;
                    case "uninstall": Uninstall(); break;
                    case "install": installEnabled = true; Reset(); break;
                    case "reboot": Reset(); initialized = false; Init(); Debug("Rebooting."); break;
                    case "contact": Debug(GetContactData()); Debug(GetCoContactData()); break;
                    case "logon_edi": ediInstalled = true; break;
                    case "logon_tmvi": tmviInstalled = true; break;
                    default: break;
                }
            }
        }
        void Delete(string _toDelete)
        {
            List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocks);
            for (int i = 0; i < _blocks.Count; i++)
            {
                _blocks[i].SetCustomName(_blocks[i].CustomName.Replace(_toDelete, ""));
            }
        }
        void Replace(string _argument)
        {
            List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocks);
            string[] _temp = _argument.Split(',');
            if (_temp.Length >= 1)
            {
                if (_temp[1].StartsWith(empty))
                    _temp[1] = _temp[1].Substring(1);
                for (int i = 0; i < _blocks.Count; i++)
                {
                    _blocks[i].SetCustomName(_blocks[i].CustomName.Replace(_temp[0], _temp[1]));
                }
            }
        }
        void Reset()
        {
            installEnabled = false;
            enabled = true;
            booted = false;
            airVentCnt = 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        /*                                                                       INITIALIZATION                                                                           */
        ////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        void _FindDebugScreen()
        {
            List<IMyTerminalBlock> _debugs = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_debugs);
                for (int i = 0; i < _debugs.Count; i++)
                {
                    if (_debugs[i].CustomName.Contains(debugID))
                    {
                        debugger.Add((IMyTextPanel)_debugs[i]);
                        VALUE_DEBUG_ENABLED = true.ToString();
                        debugEnabled = true;
                    }
                }
        }

        void _FindConfigScreen()
        {
            int _screens = 0;
            List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(_blocks);
            Debug("Textpanels found: " + _blocks.Count);
            for (int i = 0; i < _blocks.Count; i++)
            {
                Debug("Found Textpanel with name: " + _blocks[i].CustomName);
                if (_blocks[i].CustomName.Contains(configID))
                {
                    Debug("Textpanels which contains configID: " + configID);
                    _screens++; // there should`nt be more than 1!
                    config = (IMyTextPanel)_blocks[i];
                    StoreExternalConfigData();
                    config.SetValue("BackgroundColor", panelDefaultBG);
                    config.SetValue("FontColor", panelDefaultFC);
                    config.SetValue("FontSize", 0.8f);

                }
            }
            if (_screens == 0) Debug("No Configscreen found");
            
            if (_screens > 1) Debug("WARNING: TO MANY CONFIGSCREENS FOUND");
            
        }
        void _FindMyself() {
           List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();

            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(_blocks);
            for (int i = 0; i < _blocks.Count; i++)
            {
                if (_blocks[i].CustomName.Contains(coreID))
                {
                    core = (IMyProgrammableBlock)_blocks[i];
                    Debug("Found myself - Generating awareness - I know who I am. I am!");
                }
            }
        }
        void _generateConfig() 
        {
            configDict = new Dictionary<string, string>();
            configDict.Add(KEY_PLATTFORM_ID, VALUE_PLATTFORM_ID);
            configDict.Add(KEY_PLATFORM_ROLE_ID, VALUE_PLATFORM_ROLE_ID);
            configDict.Add(KEY_CONDITION, VALUE_CONDITION );
            configDict.Add(KEY_INSTALL_ENABLED, VALUE_INSTALL_ENABLED);
            configDict.Add(KEY_HIDE_VERSION_INFO, VALUE_HIDE_VERSION_INFO);
            configDict.Add(KEY_ALLOW_BEACON_RENAME, VALUE_ALLOW_BEACON_RENAME);
            configDict.Add(KEY_ACTIVATE_SECURITY, VALUE_ACTIVATE_SECURITY);
            configDict.Add(KEY_LCD_BG_COLOR, VALUE_LCD_BG_COLOR);
            configDict.Add(KEY_LCD_FONT_COLOR, VALUE_LCD_FONT_COLOR);
            configDict.Add(KEY_DEBUG_ENABLED, VALUE_DEBUG_ENABLED);
            configDict.Add(KEY_EDI_INSTALLED, VALUE_EDI_INSTALLED);
            configDict.Add(KEY_ATI_INSTALLED, VALUE_ATI_INSTALLED);
            
            initialized = true;
        }
        void Init()
        {
            
            debugMessages = new List<string>();
            debugger = new List<IMyTextPanel>();
            InitRoles();
            _FindDebugScreen();
            _FindMyself();
            assemblers = new List<IMyAssembler>();
            refineries = new List<IMyRefinery>();
            sorters = new List<IMyConveyorSorter>();
            batteries = new List<IMyBatteryBlock>();
            reactors = new List<IMyReactor>();
            oxygens = new List<IMyOxygenGenerator>();
            oxytanks = new List<IMyOxygenTank>();
            airvents = new List<IMyAirVent>();
            storage = new List<IMyInventory>();
            production = new List<IMyInventory>();
            speakers = new List<IMySoundBlock>();
            sensors = new List<IMySensorBlock>();
            lights = new List<IMyInteriorLight>();
            doors = new List<IMyDoor>();
            terminals = new List<IMyTerminalBlock>();
            panels = new List<IMyTextPanel>();
            programs = new List<IMyProgrammableBlock>();
            traders = new List<IMyProgrammableBlock>();
            timers = new List<IMyTimerBlock>();
            gatlings = new List<IMyLargeGatlingTurret>();
            missiles = new List<IMyLargeMissileTurret>();
            turrets = new List<IMyLargeInteriorTurret>();
            _FindConfigScreen();
            installEnabled = true;

        }
        void GetBlocks()
        {
            List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocks);
            for (int i = 0; i < _blocks.Count; i++)
            {
                if (_blocks[i].CustomName.Contains(platformID))
                {
                    ///////////////////// GETTING ASSEMBLERS /////////////////////
                    if (_blocks[i].CustomName.Contains(assemblerID))
                    {
                        IMyAssembler _assembler = (IMyAssembler)_blocks[i];
                        assemblers.Add(_assembler);
                        storage.Add(_assembler.GetInventory(0));
                        production.Add(_assembler.GetInventory(1));
                    }
                    ////////////////////// GETTING REFINERIES //////////////////////
                    if (_blocks[i].CustomName.Contains(refineryID))
                    {
                        IMyRefinery _refinery = (IMyRefinery)_blocks[i];
                        refineries.Add(_refinery);
                        storage.Add(_refinery.GetInventory(0));
                        production.Add(_refinery.GetInventory(1));
                    }
                    ////////////////// GETTING CONVEYOR SORTERS /////////////////
                    if (_blocks[i].CustomName.Contains(sorterID))
                        sorters.Add((IMyConveyorSorter)_blocks[i]);
                    /////////////////////// GETTING BATTERIES //////////////////////
                    if (_blocks[i].CustomName.Contains(batteryID))
                        batteries.Add((IMyBatteryBlock)_blocks[i]);
                    /////////////////////// GETTING REACTORS //////////////////////
                    if (_blocks[i].CustomName.Contains(reactorID))
                    {
                        IMyReactor _reactor = (IMyReactor)_blocks[i];
                        reactors.Add(_reactor);
                        storage.Add(_reactor.GetInventory(0));
                    }
                    /////////////////////// GETTING OXYGENS //////////////////////
                    if (_blocks[i].CustomName.Contains(oxygenID))
                    {
                        IMyOxygenGenerator _oxygen = (IMyOxygenGenerator)_blocks[i];
                        oxygens.Add(_oxygen);
                    }
                    ////////////////////// GETTING OXYTANKS //////////////////////
                    if (_blocks[i].CustomName.Contains(oxyTankID))
                    {
                        IMyOxygenTank _oxytank = (IMyOxygenTank)_blocks[i];
                        oxytanks.Add(_oxytank);
                    }
                    /////////////////////// GETTING AIRVENTS ////////////////////// 
                    if (_blocks[i].CustomName.Contains(airVentID))
                    {
                        IMyAirVent _vent = (IMyAirVent)_blocks[i];
                        airvents.Add(_vent);
                    }
                    ///////////////////// GETTING INVENTORIES /////////////////////
                    if (_blocks[i].CustomName.Contains(storageID))
                        storage.Add(((IMyInventoryOwner)_blocks[i]).GetInventory(0));
                    /////////////////// GETTING INTERIOR LIGHTS ////////////////////
                    if (_blocks[i].CustomName.Contains(lightID))
                        lights.Add((IMyInteriorLight)_blocks[i]);
                    /////////////////// GETTING SOUND BLOCKS ////////////////////
                    if (_blocks[i].CustomName.Contains(speakerID))
                        speakers.Add((IMySoundBlock)_blocks[i]);
                    /////////////////////// GETTING DOORS //////////////////////////
                    if (_blocks[i].CustomName.Contains(doorID))
                        doors.Add((IMyDoor)_blocks[i]);
                    ///////////////////// GETTING SENSORS ////////////////////////// 
                    if (_blocks[i].CustomName.Contains(sensorID))
                        sensors.Add((IMySensorBlock)_blocks[i]);
                    ///////////////////// GETTING TERMINALS ///////////////////////
                    if (_blocks[i].CustomName.Contains(terminalID))
                        terminals.Add(_blocks[i]);
                    ///////////////////// GETTING TEXT PANELS /////////////////////
                    if (_blocks[i].CustomName.Contains(panelID))
                    {
                        IMyTextPanel _panel = (IMyTextPanel)_blocks[i];
                        if (_panel.GetPublicTitle().Contains(debugID))
                        {
                            debugger.Add(_panel);
                            debugEnabled = true;
                        }
                        else
                        {
                            if (!_panel.CustomName.Contains(configID) && _panel.GetPublicTitle() == infoID)
                            {
                                _panel.ShowPublicTextOnScreen();
                                _panel.SetValue("FontSize", 0.8f);
                                panels.Add(_panel);
                            }
                        }
                    }
                    ////////////////////// GETTING PROGRAMS //////////////////////
                    if (_blocks[i].CustomName.Contains(programID))
                    {
                        IMyProgrammableBlock _program = (IMyProgrammableBlock)_blocks[i];
                        if (_program.CustomName.Contains(coreID))
                            core = _program;
                        else if (_program.CustomName.Contains(EDIID))
                            EDI = _program;
                        else if (_program.CustomName.Contains(traderID))
                            traders.Add(_program);
                        else
                            programs.Add(_program);
                    }
                    ///////////////////// GETTING TIMERBLOCKS ////////////////////
                    if (_blocks[i].CustomName.Contains(timerID))
                    {
                        if (_blocks[i].CustomName.Contains(updateID))
                        {
                            IMyTimerBlock _update = (IMyTimerBlock)_blocks[i];
                            _update.GetActionWithName("Start").Apply(_update);
                        }
                        else
                            timers.Add((IMyTimerBlock)_blocks[i]);
                    }
                    /////////////////// GETTING GATLING TURRETS /////////////////
                    if (_blocks[i].CustomName.Contains(gatlingID))
                    {
                        gatlings.Add((IMyLargeGatlingTurret)_blocks[i]);
                    }
                    if (_blocks[i].CustomName.Contains(missileID))
                    {
                        missiles.Add((IMyLargeMissileTurret)_blocks[i]);
                    }
                    if (_blocks[i].CustomName.Contains(turretID))
                    {
                        turrets.Add((IMyLargeInteriorTurret)_blocks[i]);
                    }
                }
            }
            Debug("Found: " + lights.Count + " lights");
            booted = true;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                   AUTO INSTALLATION                                                                     */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void _FormatBlock(List<IMyTerminalBlock> _blockList, string _blocktype)
        {
            string _standardText = PREFIX + platformID + DIVIDER;
            string _blockText = _standardText;
            string _oldName;
            string _blockTextEnd;
            for (int i = 0; i < _blockList.Count; i++)
            {
                _oldName = _blockList[i].CustomName;
                _blockTextEnd = suffix + empty + _oldName;
                if (!_oldName.Contains(platformID))
                {
                    switch (_blocktype)
                    {
                        case "IMyAssembler":
                            _blockText = _blockText + assemblerID + divider + productionID + _blockTextEnd; break;
                        case "IMyRefinery":
                            _blockText = _blockText + refineryID + divider + productionID + _blockTextEnd; break;
                        case "IMyConveyorSorter":
                            _blockText = _blockText + sorterID + divider + storageID + _blockTextEnd; break;
                        case "IMyBatteryBlock":
                            _blockText = _blockText + batteryID + divider + gravityID + _blockTextEnd; break;
                        case "IMyGravityGenerator":
                            _blockText = _blockText + gravityID + _blockTextEnd; break;
                        case "IMyReactor":
                            _blockText = _blockText + reactorID + divider + storageID + _blockTextEnd; break;
                        case "IMyOxygenGenerator":
                            _blockText = _blockText + oxygenID + _blockTextEnd; break;
                        case "IMyOxygenFarm":
                            _blockText = _blockText + oxyFarmID + _blockTextEnd; break;
                        case "IMyOxygenTank":
                            _blockText = _blockText + oxyTankID + _blockTextEnd; break;
                        case "IMyAirVent":
                            _blockText = _blockText + airVentID + divider + "$" + airVentCnt.ToString("0000") + _blockTextEnd;
                            airVentCnt++;
                            break;
                        case "IMyCargoContainer":
                            _blockText = _blockText + cargoID + divider + storageID + _blockTextEnd; break;
                        case "IMyShipConnector":
                            _blockText = _blockText + connectorID + divider + storageID + _blockTextEnd; break;
                        case "IMyCollector":
                            _blockText = _blockText + collectorID + divider + storageID + _blockTextEnd; break;
                        case "IMySoundBlock":
                            _blockText = _blockText + speakerID + _blockTextEnd; break;
                        case "IMyInteriorLight":
                            string _color = ((IMyInteriorLight)_blockList[i]).GetValue<Color>("Color").ToString();
                            _blockText = _blockText + lightID + divider + _color + _blockTextEnd;
                            break;
                        case "IMyDoor":
                            _blockText = _blockText + doorID + _blockTextEnd; break;
                        case "IMySensorBlock":
                            _blockText = _blockText + sensorID + _blockTextEnd; break;
                        case "IMyButtonPanel":
                            _blockText = _blockText + terminalID + _blockTextEnd; break;
                        case "IMyShipController":
                            _blockText = _blockText + terminalID + _blockTextEnd; break;
                        case "IMyTextPanel":
                            //_blockText = _blockText + panelID + _blockTextEnd; 
                            _blockText = _oldName;
                            break;
                        case "IMyProgrammableBlock":
                            //_blockText = _blockText + programID + _blockTextEnd; break;
                            _blockText =_oldName;
                            break;
                        case "IMyTimerBlock":
                            _blockText = _blockText + timerID + _blockTextEnd; break;
                        case "IMyLargeGatlingTurret":
                            _blockText = _blockText + gatlingID + _blockTextEnd; break;
                        case "IMyLargeMissileTurret":
                            _blockText = _blockText + missileID + _blockTextEnd; break;
                        case "IMyLargeInteriorTurret":
                            _blockText = _blockText + turretID + _blockTextEnd; break;
                        default:
                            _blockText = _blockText + " Unknown Block" + DIVIDER + _oldName + _blockTextEnd; break;
                    }
                    if (!_blockList[i].CustomName.Contains(platformID))
                    _blockList[i].SetCustomName(_standardText + _blockText);
                }
            }
        }
        void Install()
        {
            ///////////////////// INSTALL ASSEMBLERS /////////////////////
            List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(_blocks);
            _FormatBlock(_blocks, "IMyAssembler");
            ////////////////////// INSTALL REFINERIES //////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(_blocks);
            _FormatBlock(_blocks, "IMyRefinery");
            ////////////////// INSTALL CONVEYOR SORTERS /////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyConveyorSorter>(_blocks);
            _FormatBlock(_blocks, "IMyConveyorSorter");
            /////////////////////// INSTALL BATTERIES //////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(_blocks);
            _FormatBlock(_blocks, "IMyBatteryBlock");
            /////////////////////// INSTALL Gravity Generator //////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyGravityGenerator>(_blocks);
            _FormatBlock(_blocks, "IMyGravityGenerator");
            /////////////////////// INSTALL REACTORS //////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyReactor>(_blocks);
            _FormatBlock(_blocks, "IMyReactor");
            /////////////////////// INSTALL OXYGENS //////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyOxygenGenerator>(_blocks);
            _FormatBlock(_blocks, "IMyOxygenGenerator");
            ////////////////////// INSTALL OXYFARMS //////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyOxygenFarm>(_blocks);
            _FormatBlock(_blocks, "IMyOxygenFarm");
            ////////////////////// INSTALL OXYTANKS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyOxygenTank>(_blocks);
            _FormatBlock(_blocks, "IMyOxygenTank");
            ////////////////////// INSTALL AIRVENTS ///////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyAirVent>(_blocks);
            _FormatBlock(_blocks, "IMyAirVent");
            ///////////////////// INSTALL CONTAINERS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(_blocks);
            _FormatBlock(_blocks, "IMyCargoContainer");
            ///////////////////// INSTALL CONNECTORS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(_blocks);
            _FormatBlock(_blocks, "IMyShipConnector");
            ///////////////////// INSTALL COLLECTORS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyCollector>(_blocks);
            _FormatBlock(_blocks, "IMyCollector");
            ///////////////////// INSTALL SOUNDBLOCKS ///////////////////// 
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(_blocks);
            _FormatBlock(_blocks, "IMySoundBlock");
            /////////////////// INSTALL INTERIOR LIGHT ////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(_blocks);
            _FormatBlock(_blocks, "IMyInteriorLight");
            /////////////////////// INSTALL DOORS ///////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyDoor>(_blocks);
            _FormatBlock(_blocks, "IMyDoor");
            /////////////////////// INSTALL SENSORS ///////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(_blocks);
            _FormatBlock(_blocks, "IMySensorBlock");
            ////////////////////// INSTALL TERMINALS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyButtonPanel>(_blocks);
            _FormatBlock(_blocks, "IMyButtonPanel");
            ///////////////////// INSTALL SHIP CONTROLLS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(_blocks);
            _FormatBlock(_blocks, "IMyShipController");
            ///////////////////// INSTALL TEXT PANELS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(_blocks);
            _FormatBlock(_blocks, "IMyTextPanel");
            ////////////////// INSTALL PROGRAM BLOCKS ///////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(_blocks);
            _FormatBlock(_blocks, "IMyProgrammableBlock");
            //////////////////// INSTALL TIMER BLOCKS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(_blocks);
            _FormatBlock(_blocks, "IMyTimerBlock");
            //////////////////// INSTALL GATLING TURRETS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyLargeGatlingTurret>(_blocks);
            _FormatBlock(_blocks, "IMyLargeGatlingTurret");
            //////////////////// INSTALL MISSILE TURRETS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyLargeMissileTurret>(_blocks);
            _FormatBlock(_blocks, "IMyLargeMissileTurret");
            //////////////////// INSTALL INTERIOR TURRETS /////////////////////
            _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(_blocks);
            _FormatBlock(_blocks, "IMyLargeInteriorTurret");
            // JUST ADD NEW LINES FOR NEW BLOCKTYPES
            installEnabled = true;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                       DEINSTALLATION                                                                       */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Uninstall()
        {
            Init();
            SetCondition("green");
            Debug("Uninstalling...");
            List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocks);
            for (int i = 0; i < _blocks.Count; i++)
            {
                string _name = _blocks[i].CustomName;
                if (_name.Contains(platformID))
                {
                    string[] _temp = _name.Split(suffix);
                    if (_temp[1].StartsWith(empty))
                        _temp[1] = _temp[1].Substring(1);
                    _name = "";
                    for (int a = 1; a < _temp.Length; a++)
                    {
                        _name += _temp[a];
                    }
                    _blocks[i].SetCustomName(_name);
                }
            }
            installEnabled = false;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                           EXTERNAL DATA PROCESSING                                                            */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void LoadExternalConfigData()
        {
            // Load from screen
            string _file = config.GetPublicText();
            Debug(config.GetPublicText());
            string[] lines = _file.Split(configDivider);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = lines[i].Split(configSeparator);
                if (configDict.ContainsKey(row[0]))
                    configDict[row[0]] = row[1].Trim();
            }
            Debug(config.GetPublicText());
            StoreExternalConfigData();
        

        }
        void StoreExternalConfigData()
        {
            // write to screen
            string _file;
            _file = KEY_PLATTFORM_ID + ":" + configDict[KEY_PLATTFORM_ID] + empty + configDivider + blank;
            _file += KEY_PLATFORM_ROLE_ID + ":" + configDict[KEY_PLATFORM_ROLE_ID] + empty + configDivider + blank;
            _file += KEY_CONDITION + ":" + configDict[KEY_CONDITION] + empty + configDivider + blank;
            _file += KEY_INSTALL_ENABLED + ":" + configDict[KEY_INSTALL_ENABLED] + empty + configDivider + blank;
            _file += KEY_HIDE_VERSION_INFO + ":" + configDict[KEY_HIDE_VERSION_INFO] + empty + configDivider + blank;
            _file += KEY_ALLOW_BEACON_RENAME + ":" + configDict[KEY_ALLOW_BEACON_RENAME] + empty + configDivider + blank;
            _file += KEY_ACTIVATE_SECURITY + ":" + configDict[KEY_ACTIVATE_SECURITY] + empty + configDivider + blank;
            _file += KEY_LCD_BG_COLOR + ":" + configDict[KEY_LCD_BG_COLOR] + empty + configDivider + blank;
            _file += KEY_LCD_FONT_COLOR + ":" + configDict[KEY_LCD_FONT_COLOR] + empty + configDivider + blank;
            _file += KEY_DEBUG_ENABLED + ":" + configDict[KEY_DEBUG_ENABLED] + empty + configDivider + blank;
            _file += KEY_EDI_INSTALLED + ":" + configDict[KEY_EDI_INSTALLED] + empty + configDivider + blank;
            _file += KEY_ATI_INSTALLED + ":" + configDict[KEY_ATI_INSTALLED] + empty + configDivider + blank;
            config.WritePublicText(_file);

        }

        void Display()
        {
            string _info = GetVersionInfo();
            if (hideVersionInfoOnScreen)
                _info = "";
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].WritePublicText(_info);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                               CONDITION MANAGEMENT                                                                */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetCondition(string _condition)
        {
            condition = _condition;
            StoreExternalConfigData();
            Debug("New Condition: [" + _condition + "]");
            Debug("Switching Lights: " + lights.Count);

            if (!booted)
                GetBlocks();
            Color lcdBGColor = new Color(0, 0, 0);
            Color fontColor = new Color(255, 255, 255);
            Color lightColor = new Color(255, 255, 255);
            switch (_condition)
            {
                case "green":
                    lcdBGColor = panelDefaultBG;
                    fontColor = panelDefaultFC;
                    for (int i = 0; i < lights.Count; i++)
                    {
                        string _name = lights[i].CustomName;
                        string[] _temp = _name.Split('}');
                        _temp[0] = _temp[0].Replace(prefix + platformID + divider + lightID + divider, "").Replace("{", "");
                        string[] _colors = _temp[0].Split(' ');
                        _colors[0] = _colors[0].Replace("R:", "");
                        _colors[1] = _colors[1].Replace("G:", "");
                        _colors[2] = _colors[2].Replace("B:", "");
                        lights[i].SetValue("Color", new Color(int.Parse(_colors[0]), int.Parse(_colors[1]), int.Parse(_colors[2])));
                    }
                    break;
                case "orange":
                    lcdBGColor = conditionOrangeBG;
                    fontColor = conditionOrangeFC;
                    lightColor = conditionOrangeLight;
                    for (int i = 0; i < lights.Count; i++)
                    {
                        lights[i].SetValue("Color", lightColor);
                    }
                    break;
                case "red":
                    Debug("Switching Lights to condition red");
                    lcdBGColor = conditionRedBG;
                    fontColor = conditionRedFC;
                    lightColor = conditionRedLight;
                    Debug("Switching Lights:" + lightColor.ToString());

                    for (int i = 0; i < lights.Count; i++)
                    {
                        lights[i].SetValue("Color", lightColor);
                    }
                    break;
                default:
                    lcdBGColor = panelDefaultBG;
                    fontColor = panelDefaultFC;
                    break;
            }
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].SetValue("BackgroundColor", lcdBGColor);
                panels[i].SetValue("FontColor", fontColor);
            }
        }
        void InitRoles()
        {
            platformRoles = new List<MARole>();
            AddRole("Trading Outpost");
            AddRole("Trading Vessel");
            AddRole("Refinery");
            AddRole("Factory");
            AddRole("Production Center");
            AddRole("Service Station");
            AddRole("Shipyard");
            AddRole("Military Installation");
            AddRole("Listening Post");
            AddRole("Science Platform");
            AddRole("Observatory");
            Debug("Roles: " + platformRoles.Count + "\n");
        }
        void AddRole(string _id)
        {
            platformRoles.Add(new MARole(_id));
        }
        public struct MARole
        {
            public string ID;
            public MARole(string _id)
            {
                ID = _id;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                          DEBUGGER                                                                              */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Debug(string _msg)
        {
            debugMessages.Add(GetTimeStamp() + ": " + _msg + blank);
        }
        void DisplayDebug()
        {
            try
            {
                for (int i = 0; i < debugger.Count; i++)
                {
                    if (debugger[i] != null)
                    {
                        debugger[i].ShowPublicTextOnScreen();
                        debugger[i].SetValue("BackgroundColor", new Color(0, 0, 255));
                        debugger[i].SetValue("FontColor", new Color(255, 255, 0));
                        debugger[i].SetValue("FontSize", 0.6f);
                        debugger[i].WritePublicText(GetDebugInfo());
                    }
                    else
                        throw new ArgumentNullException("Missing Block!");
                }
            }
            catch (NullReferenceException ex)
            {
                Debug(ex.ToString());
            }
        }

        // if a Argument starts with API:
        void ProcessAPIArgument(string _argument)
        {
            Debug(_argument);
            _argument = _argument.ToLower();
            if (_argument.Contains("api: edi logon")) {
                ediInstalled = true;
                StoreExternalConfigData();                                            
            }
        } 
   }
}
