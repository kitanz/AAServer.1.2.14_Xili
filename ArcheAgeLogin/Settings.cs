namespace ArcheAgeLogin.Properties {


    // This class allows you to handle certain events in the parameter class:
    // // Event occurs Settingschanging before changing parameter value.
    // The PropertyChanged event occurs after the parameter value is changed.
    // / / The Settings Loaded event occurs after parameter values are loaded.
    // The SettingsSaving event occurs before parameter values are saved.
    internal sealed partial class Settings {
        
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the following lines:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }

        public string DataBaseConnectionString
        {
            get
            {
                string connection = "server=" + Settings.Default.DataBase_Host + ";user=" + Settings.Default.DataBase_User + ";database=" + Settings.Default.DataBase_Name + ";port=" + Settings.Default.DataBase_Port + ";password=" + Settings.Default.DataBase_Password + ";SslMode=none"; // SslMode = none";
                return connection;
            }
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code here to handle the SettingChangingEvent event.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code here to handle the SettingsSaving event.
        }
    }
}
