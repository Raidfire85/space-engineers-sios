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

namespace core
{
public class mainframe
{
/// RECOMMERED:
/// 
// To use an EXTERNAL CONFIG SCREEN you have to rename one LCD on your GRID with the following ID 
// (and only with this ID!)
const string configID = "$IOS Config";
// To use a Debugger Screen you have to set the desired Screens PUBLIC TITLE to the following ID:
const string debugID = "$IOS <CORE> Debug";
///<SUMMARY>
IMyGridTerminalSystem GridTerminalSystem;
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
const string coreID = "$IOS <CORE>";
const string EDIID = "$IOS <EDI>";
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
const char suffix = ']';
const char SUFFIX = ']';
const string prefix = "[";
const string PREFIX = "[";
bool debugEnabled = false;
bool booted = false;
bool securityActive = false;
bool initialized = false;
bool enabled = true;
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
IMyTimerBlock securityTimer;
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
    ///////////////////// INITIALIZATION /////////////////////
    if (!initialized || argument != "")
        Init();
    if (enabled)
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
    ////////////////////// DEBUGGER ///////////////////////
    if (debugEnabled)
        DisplayDebug();
    ///////////////////// RESET SCRIPT ////////////////////
    Reset();
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
void Init()
{
    debugMessages = new List<string>();
    InitRoles();
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
    debugger = new List<IMyTextPanel>();
    programs = new List<IMyProgrammableBlock>();
    traders = new List<IMyProgrammableBlock>();
    timers = new List<IMyTimerBlock>();
    gatlings = new List<IMyLargeGatlingTurret>();
    missiles = new List<IMyLargeMissileTurret>();
    turrets = new List<IMyLargeInteriorTurret>();
    List<IMyTerminalBlock> _configs = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(configID, _configs);
    if (_configs.Count > 1)
    {
        enabled = false;
        Debug("==============================================");
        Debug("CRITICAL ERROR: Detected more then 1 $IOS config!");
        Debug("Please rename or undock every $IOS config, but the ");
        Debug("One belonging to this Station!");
        Debug("==============================================");
    }
    else
    {
        config = (IMyTextPanel)_configs[0];
        config.ShowPublicTextOnScreen();
        //config.SetValue("BackgroundColor", panelDefaultBG);
        //config.SetValue("FontColor", panelDefaultFC);
        //config.SetValue("FontSize", 0.8f);
    }
    if (config == null)
    {
        Debug("No external config file found.");
        if (platformID == "")
            platformID = GetRandomPlatformID().ToString();
    }
    else
    {
        if (enabled)
        {
            Debug("External config file found! Loading DATA...");
            LoadExternalConfigData();
        }
        else
        {
            Debug("Program has been halted.");
        }
    }
    initialized = true;
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
                    _blockText = _blockText + panelID + _blockTextEnd; break;
                case "IMyProgrammableBlock":
                    _blockText = _blockText + programID + _blockTextEnd; break;
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
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////
/*                                                           EXTERNAL DATA PROCESSING                                                            */
//////////////////////////////////////////////////////////////////////////////////////////////////////////
void LoadExternalConfigData()
{
    string _file = config.GetPublicText();
    if (_file.Contains(configDivider.ToString()))
    {
        string[] _data = _file.Split(configDivider);
        //////////////////////// GETTING PLATFORM ID ////////////////////////////
        if (_data.Length > 1)
            platformID = _data[1].Replace("platformID: ", "").Replace(blank, "");
        if (platformID == "")
        {
            platformID = GetRandomPlatformID().ToString();
            _data[1] = "platformID: " + platformID;
            StoreExternalConfigData();
        }
        Debug("PlatformID: " + platformID);
        ///////////////////// GETTING PLATFORM ROLE ID /////////////////////////
        if (_data.Length > 2)
        {
            platformRoleID = int.Parse(_data[2].Replace("platformRoleID: ", "").Replace(blank, ""));
            if (platformRoles.Count > platformRoleID && platformRoleID >= 0)
                Debug("Platform Role ID: " + platformRoleID.ToString() + tab + "<" + platformRoles[platformRoleID].ID + ">");
            else
                Debug("Platform Role ID: " + platformRoleID.ToString());
        }
        //////////////////// GETTING CURRENT CONDITION /////////////////////////
        if (_data.Length > 3)
            condition = _data[3].Replace("condition: ", "").Replace(blank, "");
        Debug("Condition: " + condition);
        if (_data.Length > 4)
        {
            ////////////////////// GETTING INSTALL ENABLED /////////////////////////
            if (_data[4].ToLower().Contains("true") || _data[4].ToLower().Contains("yes"))
                installEnabled = true;
            else
                installEnabled = false;
            Debug("Install Enabled: " + installEnabled.ToString());
        }
        else    ///////////////////////// CORRECTING FORMAT
            StoreExternalConfigData();
        if (_data.Length > 5)
        {
            ///////////////////// GETTING HIDE VERSION INFO ////////////////////////
            if (_data[5].ToLower().Contains("true") || _data[5].ToLower().Contains("yes"))
                hideVersionInfoOnScreen = true;
            else
                hideVersionInfoOnScreen = false;
            Debug("Hide Version Info: " + hideVersionInfoOnScreen.ToString());
        }
        else    ///////////////////////// CORRECTING FORMAT
            StoreExternalConfigData();
        if (_data.Length > 6)
        {
            /////////////////// GETTING ALLOW BEACON RENAME //////////////////////
            if (_data[6].ToLower().Contains("true") || _data[6].ToLower().Contains("yes"))
                allowBeaconRename = true;
            else
                allowBeaconRename = false;
            Debug("Allow beacon rename: " + allowBeaconRename.ToString());
        }
        else    ///////////////////////// CORRECTING FORMAT
            StoreExternalConfigData();
        if (_data.Length > 7)
        {
            /////////////////// GETTING SECURITY ACTIVE //////////////////////
            if (_data[7].ToLower().Contains("true") || _data[7].ToLower().Contains("yes"))
                securityActive = true;
            else
                securityActive = false;
            Debug("securityActive: " + securityActive.ToString());
        }
        else    ///////////////////////// CORRECTING FORMAT
            StoreExternalConfigData();
        if (_data.Length > 9)
        {
            ///////////////////////// GET DEFAULT PANEL BG COLOR
            string[] _temp = _data[8].Split('}');
            _temp[0] = _temp[0].Replace("LCD BG Color: {", "");
            string[] _colors = _temp[0].Split(' ');
            _colors[0] = _colors[0].Replace("R:", "");
            _colors[1] = _colors[1].Replace("G:", "");
            _colors[2] = _colors[2].Replace("B:", "");
            panelDefaultBG = new Color(int.Parse(_colors[0]), int.Parse(_colors[1]), int.Parse(_colors[2]));
            ///////////////////////// GET DEFAULT PANEL FONT COLOR
            _temp = _data[9].Split('}');
            _temp[0] = _temp[0].Replace("LCD Font Color: {", "");
            _colors = _temp[0].Split(' ');
            _colors[0] = _colors[0].Replace("R:", "");
            _colors[1] = _colors[1].Replace("G:", "");
            _colors[2] = _colors[2].Replace("B:", "");
            panelDefaultFC = new Color(int.Parse(_colors[0]), int.Parse(_colors[1]), int.Parse(_colors[2]));
        }
        else
        {
            StoreExternalConfigData();
        }
        if (_data.Length > 10)
        {
            Debug("Debug Enabled: " + debugEnabled.ToString());
        }
    }
    else
    {
        ///////////////////////// CORRECTING FORMAT
        StoreExternalConfigData();
    }
    SetCondition(condition);
}
void StoreExternalConfigData()
{
    string _file = "$platformID: " + platformID + blank;
    _file += "$platformRoleID: " + platformRoleID.ToString() + blank;
    _file += "$condition: " + condition + blank;
    _file += "$install enabled: " + installEnabled.ToString() + blank;
    _file += "$hide version info: " + hideVersionInfoOnScreen.ToString() + blank;
    _file += "$allow beacon rename: " + allowBeaconRename.ToString() + blank;
    _file += "$activate security: " + securityActive.ToString() + blank;
    _file += "$LCD BG Color: " + panelDefaultBG + blank;
    _file += "$LCD Font Color: " + panelDefaultFC + blank;
    _file += "$Debug Enabled: " + debugEnabled.ToString() + blank;
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
            lcdBGColor = conditionRedBG;
            fontColor = conditionRedFC;
            lightColor = conditionRedLight;
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
}
}
