# space-engineers-sios
		
		Self installing Operation System ($IOS) - Made by Mahtrok

        /// RECOMMERED:
        /// 
        // To use an EXTERNAL CONFIG SCREEN you have to rename one LCD on your GRID with the following ID 
        // (and only with this ID!)
        const string configID = "$IOS Config";

        // To use a Debugger Screen you have to set the desired Screens PUBLIC TITLE to the following ID:
        const string debugID = "$IOS <CORE> Debug";


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
