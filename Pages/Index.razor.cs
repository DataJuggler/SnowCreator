

#region using statements

using DataJuggler.Blazor.Components;
using DataJuggler.Blazor.Components.Interfaces;
using DataJuggler.Blazor.Components.Util;
using DataJuggler.RandomUSD;
using DataJuggler.UltimateHelper;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.ComponentModel;
using System.IO.Compression;

#endregion

namespace SnowCreator.Pages
{

    #region class Index
    /// <summary>
    /// This class is trhe main page for this app.
    /// </summary>
    public partial class Index : IBlazorComponentParent, ISpriteSubscriber
    {
        
        #region Private Variables
        private List<IBlazorComponent> children;
        private ValidationComponent translateXMinControl;
        private ValidationComponent translateXMaxControl;
        private ValidationComponent translateYMinControl;
        private ValidationComponent translateYMaxControl;
        private ValidationComponent translateZMinControl;
        private ValidationComponent translateZMaxControl;
        private ValidationComponent scaleMinControl;
        private ValidationComponent scaleMaxControl;
        private ValidationComponent additionalYControl;
        private ValidationComponent objectsToCreateControl;
        private Label statusLabel;
        private string downloadFileName;
        private string downloadPath;
        private Settings settings;
        private bool showProgress;
        private Sprite invisibleSprite;
        private BackgroundWorker worker;
        private string progressStyle; 
        private string percentString;
        private int percent;        
        private string labelColor;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Create a new instance of an Index object
        /// </summary>
        public Index()
        {
            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Events

            #region OnAfterRenderAsync(bool firstRender)
            /// <summary>
            /// This method is used to verify a user
            /// </summary>
            /// <param name="firstRender"></param>
            /// <returns></returns>
            protected async override Task OnAfterRenderAsync(bool firstRender)
            {
                // if the value for HasSettings is true
                if ((HasSettings) && (firstRender))
                {
                    // if the component exists
                    if (HasAdditionalYControl)
                    {
                        // Set the value
                        AdditionalYControl.SetTextValue(Settings.AdditionalY.ToString());
                    }

                    // if the component exists
                    if (HasObjectsToCreateControl)
                    {
                        // Set the value
                        ObjectsToCreateControl.SetTextValue(Settings.ObjectsToCreate.ToString());
                    }

                    // if the component exists
                    if (HasScaleMaxControl)
                    {
                        // Set the value
                        ScaleMaxControl.SetTextValue(Settings.ScaleMax.ToString());
                    }

                    // if the component exists
                    if (HasScaleMinControl)
                    {
                        // Set the value
                        ScaleMinControl.SetTextValue(Settings.ScaleMin.ToString());
                    }

                    // if the component exists
                    if (HasTranslateXMaxControl)
                    {
                        // Set the value
                        TranslateXMaxControl.SetTextValue(Settings.TranslateXMax.ToString());
                    }

                    // if the component exists
                    if (HasTranslateXMinControl)
                    {
                        // Set the value
                        TranslateXMinControl.SetTextValue(Settings.TranslateXMin.ToString());
                    }

                    // if the component exists
                    if (HasTranslateYMaxControl)
                    {
                        // Set the value
                        TranslateYMaxControl.SetTextValue(Settings.TranslateYMax.ToString());
                    }

                    // if the component exists
                    if (HasTranslateYMinControl)
                    {
                        // Set the value
                        TranslateYMinControl.SetTextValue(Settings.TranslateYMin.ToString());
                    }

                    // if the component exists
                    if (HasTranslateZMaxControl)
                    {
                        // Set the value
                        TranslateZMaxControl.SetTextValue(Settings.TranslateZMax.ToString());
                    }

                    // if the component exists
                    if (HasTranslateZMinControl)
                    {
                        // Set the value
                        TranslateZMinControl.SetTextValue(Settings.TranslateZMin.ToString());
                    }
                }

                // call the base
                await base.OnAfterRenderAsync(firstRender);
            }
            #endregion

            #region Worker_DoWork(object sender, DoWorkEventArgs e)
            /// <summary>
            /// event is fired when Worker _ Do Work
            /// </summary>
            private async void Worker_DoWork(object sender, DoWorkEventArgs e)
            {
                try
                {
                    // Perform the randomization
                    RandomizationResult result = await Randomizer.RandomizeAsync(settings);

                    // return the result
                    e.Result = result;

                    RunWorkerCompletedEventArgs eventArgs = new RunWorkerCompletedEventArgs(result, result.Error, false);

                    // Run the completed code now
                    Worker_RunWorkerCompleted(this, eventArgs);
                }
                catch (Exception error)
                {
                    // for debugging only
                    DebugHelper.WriteDebugError("Worker_DoWork", "Index.razor.cs", error);

                    // Set the error
                    e.Result = error;
                }
            }
            #endregion

            #region Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            /// <summary>
            /// event is fired when Worker _ Run Worker Completed
            /// </summary>
            private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                // local
                RandomizationResult result = null;

                try
                {
                    // check if there is a result
                    result = e.Result as RandomizationResult;

                    // if this is the result
                    if (NullHelper.Exists(result))
                    {
                        // did it work?
                        if (result.Success)
                        {
                            // Create a name for the zip file
                            string newFileName = result.OutputFileName.Replace(".usda", ".zip");

                            // Create a new instance of a 'FileInfo' object.
                            FileInfo fileInfo = new FileInfo(result.OutputFileName);

                            // reference System.IO.Compression
                            using (var zip = ZipFile.Open(newFileName, ZipArchiveMode.Create))
                            {  
                                zip.CreateEntryFromFile(result.OutputFileName, fileInfo.Name);

                                // Delete the .cs file
                                File.Delete(result.OutputFileName);
                            }

                            // Create a new instance of a 'FileInfo' object.
                            FileInfo fileInfo2 = new FileInfo(newFileName);

                            // Set the DownloadPath
                            DownloadPath = "../Downloads/SnowScenes/" + fileInfo2.Name; // EnvironmentVariableHelper.GetEnvironmentVariableValue("SnowCreatorURL", EnvironmentVariableTarget.Machine) + "wwwroot/Downloads/SnowScenes/" + fileInfo2.Name;

                             // Set the Download Filename
                            DownloadFileName = fileInfo2.Name;

                            // Setup the label
                            StatusLabel.SetTextValue("This download will only be available for the next hour");

                            // Stop showing the progress bar
                            ShowProgress = false;
                        }
                        else
                        {
                            // if an error occurred
                            if (result.HasError)
                            {
                                // for debugging only for now
                                string error = result.Error.ToString();
                            }

                            // Setup the label
                            StatusLabel.SetTextValue("An error occurred creating your snow scene.");
                        }
                    }

                    // Update the UI
                    Refresh();
                }
                catch (Exception error)
                {
                    // log the error
                    DebugHelper.WriteDebugError("Worker_RunWorkerCompleted", "Index.razor.cs", error);
                }
                finally
                {   
                    // do not remove this the first time
                    if (NullHelper.Exists(result))
                    {
                        // Erase the DoWork
                        Worker.DoWork -= Worker_DoWork;

                        // Erase the Completed method
                        Worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;

                        // dispose of the worker
                        Worker.Dispose();

                        // destory the reference
                        Worker = null;
                    }
                }
            }
            #endregion

        #endregion
        
        #region Methods

            #region ButtonClicked(int buttonNumber, string buttonText)
            /// <summary>
            /// This method serves as the ClickHandler for buttons
            /// </summary>
            /// <param name="buttonNumber"></param>
            /// <param name="buttonText"></param>
            public void ButtonClicked(int buttonNumber, string buttonText)
            {
                // Reset
                Percent = 0;
                LabelColor = "black";

                // if the component exists
                if (HasAdditionalYControl)
                {
                    // Set the value
                    Settings.AdditionalY = NumericHelper.ParseInteger(AdditionalYControl.Text, 0, 0);
                }

                // if the component exists
                if (HasObjectsToCreateControl)
                {
                    // Set the value
                    Settings.ObjectsToCreate = NumericHelper.ParseInteger(ObjectsToCreateControl.Text, 0, 0);
                }

                // if the component exists
                if (HasScaleMaxControl)
                {
                    // Set the value
                    Settings.ScaleMax = NumericHelper.ParseInteger(ScaleMaxControl.Text, 0, 0);
                }

                // if the component exists
                if (HasScaleMinControl)
                {
                    // Set the value
                    Settings.ScaleMin = NumericHelper.ParseInteger(ScaleMinControl.Text, 0, 0);
                }

                // if the component exists
                if (HasTranslateXMaxControl)
                {
                    // Set the value
                    Settings.TranslateXMax = NumericHelper.ParseInteger(TranslateXMaxControl.Text, 0, 0);
                }

                // if the component exists
                if (HasTranslateXMinControl)
                {
                    // Set the value
                    Settings.TranslateXMin = NumericHelper.ParseInteger(TranslateXMinControl.Text, 0, 0);
                }

                // if the component exists
                if (HasTranslateYMaxControl)
                {
                    // Set the value
                    Settings.TranslateYMax = NumericHelper.ParseInteger(TranslateYMaxControl.Text, 0, 0);
                }

                // if the component exists
                if (HasTranslateYMinControl)
                {
                    // Set the value
                    Settings.TranslateYMin = NumericHelper.ParseInteger(TranslateYMinControl.Text, 0, 0);
                }

                // if the component exists
                if (HasTranslateZMaxControl)
                {
                    // Set the value
                    Settings.TranslateZMax = NumericHelper.ParseInteger(TranslateZMaxControl.Text, 0, 0);
                }

                // if the component exists
                if (HasTranslateZMinControl)
                {
                    // Set the value
                    Settings.TranslateZMin = NumericHelper.ParseInteger(TranslateZMinControl.Text, 0, 0);
                }

                // needed for validation messages
                string invalidReason = "";

                // Check if the settings are valid
                bool isValid = ValidateSettings(ref invalidReason);

                // if the settings validate
                if (isValid)
                {
                    // Show the Progressbar
                    ShowProgress = true;

                    // if the ProgressBar
                    if (HasInvisibleSprite)
                    {
                        // Start the Timer
                        InvisibleSprite.Start();
                    }

                    // Show a message
                    StatusLabel.SetTextValue("Creating your snow scene, please wait...");

                    // Erase
                    DownloadFileName = "";

                    // Create the Worker
                    Worker = new BackgroundWorker();

                    // Setup the DoWork
                    Worker.DoWork += Worker_DoWork;

                    // Setup the Completed method
                    Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

                    // Start
                    Worker.RunWorkerAsync(Settings);
                }
                else
                {
                    // change to a red color
                    LabelColor = "firebrick";

                    // Show the user a message
                    StatusLabel.SetTextValue(invalidReason);
                }

                // Update UI
                Refresh();
            }
            #endregion
            
            #region FindChildByName(string name)
            /// <summary>
            /// method Find Child By Name
            /// </summary>
            public IBlazorComponent FindChildByName(string name)
            {
                // probably not used
                return ComponentHelper.FindChildByName(Children, name);
            }
            #endregion
            
            #region Init()
            /// <summary>
            ///  This method performs initializations for this object.
            /// </summary>
            public void Init()
            {
                // Create a new instance of a 'Settings' object.
                Settings = new Settings();

                // Set the settings
                Settings.ObjectTagName = "[SphereName]";
                Settings.ObjectName = "Sphere";
                Settings.LineBeforeLastBracketText = "xformOpOrder";
                Settings.EmptyUSDPath = "wwwroot/Downloads/Templates/SnowEmpy.usda";
                Settings.OutputFolderPath = "wwwroot/Downloads/SnowScenes";
                Settings.OutputFileName = "SnowScene.usda";
                Settings.ObjectsToCreate = 4000;
                Settings.SmallScale = true;
                Settings.TranslateXMin = -500;
                Settings.TranslateXMax = 500;
                Settings.TranslateYMin = 500;
                Settings.TranslateYMax = 5000;
                Settings.TranslateZMin = -500;
                Settings.TranslateZMax = 500;
                Settings.AdditionalY = 0;
                Settings.ScaleMax = 5;
                Settings.ScaleMin = 1;

                //
                LabelColor = "black";
            }
            #endregion
            
            #region ReceiveData(Message message)
            /// <summary>
            /// method Receive Data
            /// </summary>
            public void ReceiveData(Message message)
            {
                
            }
            #endregion
            
            #region Refresh()
            /// <summary>
            /// method Refresh
            /// </summary>
            public void Refresh()
            {
                // if the value for ShowProgress is true
                if (ShowProgress)
                {
                    // increment by 4
                    Percent += 4;

                    // go a little past 100 for effect
                    if (Percent >= 100)
                    {
                        // Stop the timer
                        InvisibleSprite.Stop();
                        ShowProgress = false;
                    }
                }

                // Update the UI
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            #endregion
            
            #region Register(Sprite sprite)
            /// <summary>
            /// method returns the
            /// </summary>
            public void Register(Sprite sprite)
            {
                // store the InvisibleSprite, used for the progerss bar
                InvisibleSprite = sprite;
            }
            #endregion
            
            #region Register(IBlazorComponent component)
            /// <summary>
            /// method Register
            /// </summary>
            public void Register(IBlazorComponent component)
            {
                if (component is ValidationComponent)
                {
                    switch (component.Name)
                    {
                        case "AdditionalY":

                            // Store the control
                            AdditionalYControl = component as ValidationComponent;

                            // required
                            break;

                        case "ObjectsToCreate":

                            // Store the control
                            ObjectsToCreateControl = component as ValidationComponent;

                            // required
                            break;

                        case "ScaleMax":

                            // Store the control
                            ScaleMaxControl = component as ValidationComponent;

                            // required
                            break;

                        case "ScaleMin":

                            // Store the control
                            ScaleMinControl = component as ValidationComponent;

                            // required
                            break;

                        case "TranslateXMin":

                            // Store the control
                            TranslateXMinControl = component as ValidationComponent;

                            // required
                            break;

                        case "TranslateXMax":

                            // Store the control
                            TranslateXMaxControl = component as ValidationComponent;

                            // required
                            break;

                        case "TranslateYMin":

                            // Store the control
                            TranslateYMinControl = component as ValidationComponent;

                            // required
                            break;

                        case "TranslateYMax":

                            // Store the control
                            TranslateYMaxControl = component as ValidationComponent;

                            // required
                            break;

                        case "TranslateZMin":

                            // Store the control
                            TranslateZMinControl = component as ValidationComponent;

                            // required
                            break;

                        case "TranslateZMax":

                            // Store the control
                            TranslateZMaxControl = component as ValidationComponent;

                            // required
                            break;
                    }
                }
                else if (component is Label)
                {
                    // Store the status label
                    StatusLabel = component as Label;
                }
            }
            #endregion

            #region ResetControls()
            /// <summary>
            /// Reset Controls
            /// </summary>
            public void ResetControls()
            {
                // if the component exists
                if (HasAdditionalYControl)
                {
                    // Set the value for IsValid
                    AdditionalYControl.IsValid = true;

                    // Resets them all?
                    AdditionalYControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasObjectsToCreateControl)
                {
                    // Set the value for IsValid
                    ObjectsToCreateControl.IsValid = true;

                    // reset
                    ObjectsToCreateControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasScaleMaxControl)
                {
                    // Set the value for IsValid
                    ScaleMaxControl.IsValid = true;

                    // reset
                    ScaleMaxControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasScaleMinControl)
                {
                    // Set the value for IsValid
                    ScaleMinControl.IsValid = true;

                    // reset
                    ScaleMinControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasTranslateXMaxControl)
                {
                    // Set the value for IsValid
                    TranslateXMaxControl.IsValid = true;

                    // reset
                    TranslateXMaxControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasTranslateXMinControl)
                {
                    // Set the value for IsValid
                    TranslateXMinControl.IsValid = true;

                    // reset
                    TranslateXMinControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasTranslateYMaxControl)
                {
                    // Set the value for IsValid
                    TranslateYMaxControl.IsValid = true;

                    // reset
                    TranslateYMaxControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasTranslateYMinControl)
                {
                    // Set the value for IsValid
                    TranslateYMinControl.IsValid = true;

                    // reset
                    TranslateYMinControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasTranslateZMaxControl)
                {
                    // Set the value for IsValid
                    TranslateZMaxControl.IsValid = true;

                    // reset
                    TranslateZMaxControl.SetInvalidLabelColor("black");
                }

                // if the component exists
                if (HasTranslateZMinControl)
                {
                    // Set the value for IsValid
                    TranslateZMinControl.IsValid = true;

                    // reset
                    TranslateZMinControl.SetInvalidLabelColor("black");
                }

                // Update
                Refresh();
            }
            #endregion
            
            #region ValidateSettings(ref string invalidReason)
            /// <summary>
            /// returns the Settings
            /// </summary>
            public bool ValidateSettings(ref string invalidReason)
            {
                // initial value
                bool isValid = false;

                // if the value for HasSettings is true
                if (HasSettings)
                {
                    // Reset all controls in case any were invalid
                    ResetControls();

                    // if too many
                    if (settings.ObjectsToCreate > 6000)
                    {
                        // set the invalid reason
                        invalidReason = "Objects to Create must be between 1 and 6,000";

                        // if the value for HasObjectsToCreateControl is true
                        if (HasObjectsToCreateControl)
                        {  
                            ObjectsToCreateControl.IsValid = false;
                        }
                    }
                    else if (settings.ObjectsToCreate < 1)
                    {
                        // set the invalid reason
                        invalidReason = "Objects to Create must be between 1 and 6,000";

                        // if the value for HasObjectsToCreateControl is true
                        if (HasObjectsToCreateControl)
                        {  
                            ObjectsToCreateControl.IsValid = false;
                        }
                    }
                    else if (settings.TranslateXMax - settings.TranslateXMin > 10000)
                    {
                        // set the invalid reason
                        invalidReason = "Translate X Max - X Min must be less than 10,000";

                        // if the value for HasTranslateXMaxControl is true
                        if (HasTranslateXMaxControl)
                        {
                            TranslateXMaxControl.IsValid = false;                            
                        }

                        // if the value for HasTranslateXMinControl is true
                        if (HasTranslateXMinControl)
                        {
                            TranslateXMinControl.IsValid = false;                            
                        }
                    }
                    else if (settings.TranslateXMax < settings.TranslateXMin)
                    {
                        // set the invalid reason
                        invalidReason = "Translate X Max must be greater than Translate X Min";

                        // if the value for HasTranslateXMaxControl is true
                        if (HasTranslateXMaxControl)
                        {
                            TranslateXMaxControl.IsValid = false;                            
                        }

                        // if the value for HasTranslateXMinControl is true
                        if (HasTranslateXMinControl)
                        {
                            TranslateXMinControl.IsValid = false;                            
                        }
                    }
                    else if (settings.TranslateZMax - settings.TranslateZMin > 10000)
                    {
                        // set the invalid reason
                        invalidReason = "Translate Z Max - Z Min must be less than 10,000";

                        // if the value for HasTranslateZMaxControl is true
                        if (HasTranslateZMaxControl)
                        {
                            TranslateZMaxControl.IsValid = false;                            
                        }

                        // if the value for HasTranslateZMinControl is true
                        if (HasTranslateZMinControl)
                        {
                            TranslateZMinControl.IsValid = false;                            
                        }
                    }
                    else if (settings.TranslateZMax < settings.TranslateZMin)
                    {
                        // set the invalid reason
                        invalidReason = "Translate Z Max must be greater than Translate Z Min";

                        // if the value for HasTranslateZMaxControl is true
                        if (HasTranslateZMaxControl)
                        {
                            TranslateZMaxControl.IsValid = false;                            
                        }

                        // if the value for HasTranslateZMinControl is true
                        if (HasTranslateZMinControl)
                        {
                            TranslateZMinControl.IsValid = false;                            
                        }
                    }
                    else if (settings.TranslateYMax < settings.TranslateYMin)
                    {
                        // set the invalid reason
                        invalidReason = "Translate Y Max must be greater than Translate Y Min";

                        // if the value for HasTranslateYMaxControl is true
                        if (HasTranslateYMaxControl)
                        {
                            TranslateYMaxControl.IsValid = false;                            
                        }

                        // if the value for HasTranslateYMinControl is true
                        if (HasTranslateYMinControl)
                        {
                            TranslateYMinControl.IsValid = false;                            
                        }
                    }
                    else if (settings.TranslateYMax > 999999)
                    {
                        // set the invalid reason
                        invalidReason = "Translate Y Max must be less than 999,999";

                        // if the value for HasTranslateYMaxControl is true
                        if (HasTranslateYMaxControl)
                        {
                            TranslateYMaxControl.IsValid = false;                            
                        }
                    }
                    else if (settings.TranslateYMin < 0)
                    {
                        // set the invalid reason
                        invalidReason = "Translate Y Min must be a positiive number.";

                         // if the value for HasTranslateYMinControl is true
                        if (HasTranslateYMinControl)
                        {
                            TranslateYMinControl.IsValid = false;                            
                        }
                    }
                    else if (settings.ScaleMin <= 0)
                    {
                        // set the invalid reason
                        invalidReason = "Scale Min must be a positiive number.";

                        // if the value for HasScaleMinControl is true
                        if (HasScaleMinControl)
                        {
                            ScaleMinControl.IsValid = false;                            
                        }
                    }
                    else if (settings.ScaleMin > settings.ScaleMax)
                    {
                        // set the invalid reason
                        invalidReason = "Scale Min must be less than Scale Max.";

                        // if the value for HasScaleMinControl is true
                        if (HasScaleMinControl)
                        {
                            ScaleMinControl.IsValid = false;                            
                        }

                        // if the value for HasScaleMaxControl is true
                        if (HasScaleMaxControl)
                        {
                            ScaleMaxControl.IsValid = false;                            
                        }
                    }
                    else if (settings.ScaleMax > 9)
                    {
                        // set the invalid reason
                        invalidReason = "Scale Max must be between 1 and 9";

                        // if the value for HasScaleMaxControl is true
                        if (HasScaleMaxControl)
                        {
                            ScaleMaxControl.IsValid = false;                            
                        }
                    }
                    else if (settings.AdditionalY > 999999)
                    {
                        // set the invalid reason
                        invalidReason = "Additional Y must be less than 1,000,000";

                        // if the value for HasAdditionalYControl is true
                        if (HasAdditionalYControl)
                        {
                            AdditionalYControl.IsValid = false;                            
                        }
                    }
                    else
                    {
                        // validates
                        isValid = true;
                    }
                }
                else
                {
                    // Should never happen
                    invalidReason = "Internal Error. Settings does not exist";
                }
                
                // return value
                return isValid;
            }
            #endregion
            
        #endregion

        #region Properties

            #region AdditionalYControl
            /// <summary>
            /// This property gets or sets the value for 'AdditionalYControl'.
            /// </summary>
            public ValidationComponent AdditionalYControl
            {
                get { return additionalYControl; }
                set { additionalYControl = value; }
            }
            #endregion
            
            #region Children
            /// <summary>
            /// This property gets or sets the value for 'Children'.
            /// </summary>
            public List<IBlazorComponent> Children
            {
                get { return children; }
                set { children = value; }
            }
            #endregion
            
            #region DownloadFileName
            /// <summary>
            /// This property gets or sets the value for 'DownloadFileName'.
            /// </summary>
            public string DownloadFileName
            {
                get { return downloadFileName; }
                set { downloadFileName = value; }
            }
            #endregion
            
            #region DownloadPath
            /// <summary>
            /// This property gets or sets the value for 'DownloadPath'.
            /// </summary>
            public string DownloadPath
            {
                get { return downloadPath; }
                set { downloadPath = value; }
            }
            #endregion
            
            #region HasAdditionalYControl
            /// <summary>
            /// This property returns true if this object has an 'AdditionalYControl'.
            /// </summary>
            public bool HasAdditionalYControl
            {
                get
                {
                    // initial value
                    bool hasAdditionalYControl = (this.AdditionalYControl != null);
                    
                    // return value
                    return hasAdditionalYControl;
                }
            }
            #endregion
            
            #region HasChildren
            /// <summary>
            /// This property returns true if this object has a 'Children'.
            /// </summary>
            public bool HasChildren
            {
                get
                {
                    // initial value
                    bool hasChildren = (this.Children != null);
                    
                    // return value
                    return hasChildren;
                }
            }
            #endregion
            
            #region HasDownloadFileName
            /// <summary>
            /// This property returns true if the 'DownloadFileName' exists.
            /// </summary>
            public bool HasDownloadFileName
            {
                get
                {
                    // initial value
                    bool hasDownloadFileName = (!String.IsNullOrEmpty(this.DownloadFileName));
                    
                    // return value
                    return hasDownloadFileName;
                }
            }
            #endregion
            
            #region HasInvisibleSprite
            /// <summary>
            /// This property returns true if this object has an 'InvisibleSprite'.
            /// </summary>
            public bool HasInvisibleSprite
            {
                get
                {
                    // initial value
                    bool hasInvisibleSprite = (this.InvisibleSprite != null);
                    
                    // return value
                    return hasInvisibleSprite;
                }
            }
            #endregion
            
            #region HasObjectsToCreateControl
            /// <summary>
            /// This property returns true if this object has an 'ObjectsToCreateControl'.
            /// </summary>
            public bool HasObjectsToCreateControl
            {
                get
                {
                    // initial value
                    bool hasObjectsToCreateControl = (this.ObjectsToCreateControl != null);
                    
                    // return value
                    return hasObjectsToCreateControl;
                }
            }
            #endregion
            
            #region HasScaleMaxControl
            /// <summary>
            /// This property returns true if this object has a 'ScaleMaxControl'.
            /// </summary>
            public bool HasScaleMaxControl
            {
                get
                {
                    // initial value
                    bool hasScaleMaxControl = (this.ScaleMaxControl != null);
                    
                    // return value
                    return hasScaleMaxControl;
                }
            }
            #endregion
            
            #region HasScaleMinControl
            /// <summary>
            /// This property returns true if this object has a 'ScaleMinControl'.
            /// </summary>
            public bool HasScaleMinControl
            {
                get
                {
                    // initial value
                    bool hasScaleMinControl = (this.ScaleMinControl != null);
                    
                    // return value
                    return hasScaleMinControl;
                }
            }
            #endregion
            
            #region HasSettings
            /// <summary>
            /// This property returns true if this object has a 'Settings'.
            /// </summary>
            public bool HasSettings
            {
                get
                {
                    // initial value
                    bool hasSettings = (this.Settings != null);
                    
                    // return value
                    return hasSettings;
                }
            }
            #endregion
            
            #region HasStatusLabel
            /// <summary>
            /// This property returns true if this object has a 'StatusLabel'.
            /// </summary>
            public bool HasStatusLabel
            {
                get
                {
                    // initial value
                    bool hasStatusLabel = (this.StatusLabel != null);
                    
                    // return value
                    return hasStatusLabel;
                }
            }
            #endregion
            
            #region HasTranslateXMaxControl
            /// <summary>
            /// This property returns true if this object has a 'TranslateXMaxControl'.
            /// </summary>
            public bool HasTranslateXMaxControl
            {
                get
                {
                    // initial value
                    bool hasTranslateXMaxControl = (this.TranslateXMaxControl != null);
                    
                    // return value
                    return hasTranslateXMaxControl;
                }
            }
            #endregion
            
            #region HasTranslateXMinControl
            /// <summary>
            /// This property returns true if this object has a 'TranslateXMinControl'.
            /// </summary>
            public bool HasTranslateXMinControl
            {
                get
                {
                    // initial value
                    bool hasTranslateXMinControl = (this.TranslateXMinControl != null);
                    
                    // return value
                    return hasTranslateXMinControl;
                }
            }
            #endregion
            
            #region HasTranslateYMaxControl
            /// <summary>
            /// This property returns true if this object has a 'TranslateYMaxControl'.
            /// </summary>
            public bool HasTranslateYMaxControl
            {
                get
                {
                    // initial value
                    bool hasTranslateYMaxControl = (this.TranslateYMaxControl != null);
                    
                    // return value
                    return hasTranslateYMaxControl;
                }
            }
            #endregion
            
            #region HasTranslateYMinControl
            /// <summary>
            /// This property returns true if this object has a 'TranslateYMinControl'.
            /// </summary>
            public bool HasTranslateYMinControl
            {
                get
                {
                    // initial value
                    bool hasTranslateYMinControl = (this.TranslateYMinControl != null);
                    
                    // return value
                    return hasTranslateYMinControl;
                }
            }
            #endregion
            
            #region HasTranslateZMaxControl
            /// <summary>
            /// This property returns true if this object has a 'TranslateZMaxControl'.
            /// </summary>
            public bool HasTranslateZMaxControl
            {
                get
                {
                    // initial value
                    bool hasTranslateZMaxControl = (this.TranslateZMaxControl != null);
                    
                    // return value
                    return hasTranslateZMaxControl;
                }
            }
            #endregion
            
            #region HasTranslateZMinControl
            /// <summary>
            /// This property returns true if this object has a 'TranslateZMinControl'.
            /// </summary>
            public bool HasTranslateZMinControl
            {
                get
                {
                    // initial value
                    bool hasTranslateZMinControl = (this.TranslateZMinControl != null);
                    
                    // return value
                    return hasTranslateZMinControl;
                }
            }
            #endregion
            
            #region InvisibleSprite
            /// <summary>
            /// This property gets or sets the value for 'InvisibleSprite'.
            /// </summary>
            public Sprite InvisibleSprite
            {
                get { return invisibleSprite; }
                set { invisibleSprite = value; }
            }
            #endregion
            
            #region LabelColor
            /// <summary>
            /// This property gets or sets the value for 'LabelColor'.
            /// </summary>
            public string LabelColor
            {
                get { return labelColor; }
                set { labelColor = value; }
            }
            #endregion
            
            #region ObjectsToCreateControl
            /// <summary>
            /// This property gets or sets the value for 'ObjectsToCreateControl'.
            /// </summary>
            public ValidationComponent ObjectsToCreateControl
            {
                get { return objectsToCreateControl; }
                set { objectsToCreateControl = value; }
            }
            #endregion

            #region Percent
            /// <summary>
            /// This property gets or sets the value for 'Percent'.
            /// </summary>
            public int Percent
            {
                get { return percent; }
                set 
                {
                    // if less than zero
                    if (value < 0)
                    {
                        // set to 0
                        value = 0;
                    }

                    // if greater than 100
                    if (value > 100)
                    {
                        // set to 100
                        value = 100;
                    }

                    // set the value
                    percent = value;

                    // Now set ProgressStyle
                    ProgressStyle = "c100 p[Percent] dark small orange".Replace("[Percent]", percent.ToString());

                    // Set the percentString value
                    PercentString = percent.ToString() + "%";
                }
            }
            #endregion
            
            #region PercentString
            /// <summary>
            /// This property gets or sets the value for 'PercentString'.
            /// </summary>
            public string PercentString
            {
                get { return percentString; }
                set { percentString = value; }
            }
            #endregion
            
            #region ProgressStyle
            /// <summary>
            /// This property gets or sets the value for 'ProgressStyle'.
            /// </summary>
            public string ProgressStyle
            {
                get { return progressStyle; }
                set { progressStyle = value; }
            }
            #endregion
            
            #region ScaleMaxControl
            /// <summary>
            /// This property gets or sets the value for 'ScaleMaxControl'.
            /// </summary>
            public ValidationComponent ScaleMaxControl
            {
                get { return scaleMaxControl; }
                set { scaleMaxControl = value; }
            }
            #endregion
            
            #region ScaleMinControl
            /// <summary>
            /// This property gets or sets the value for 'ScaleMinControl'.
            /// </summary>
            public ValidationComponent ScaleMinControl
            {
                get { return scaleMinControl; }
                set { scaleMinControl = value; }
            }
            #endregion
            
            #region Settings
            /// <summary>
            /// This property gets or sets the value for 'Settings'.
            /// </summary>
            public Settings Settings
            {
                get { return settings; }
                set { settings = value; }
            }
            #endregion
            
            #region ShowProgress
            /// <summary>
            /// This property gets or sets the value for 'ShowProgress'.
            /// </summary>
            public bool ShowProgress
            {
                get { return showProgress; }
                set { showProgress = value; }
            }
            #endregion
            
            #region StatusLabel
            /// <summary>
            /// This property gets or sets the value for 'StatusLabel'.
            /// </summary>
            public Label StatusLabel
            {
                get { return statusLabel; }
                set { statusLabel = value; }
            }
            #endregion
            
            #region TranslateXMaxControl
            /// <summary>
            /// This property gets or sets the value for 'TranslateXMaxControl'.
            /// </summary>
            public ValidationComponent TranslateXMaxControl
            {
                get { return translateXMaxControl; }
                set { translateXMaxControl = value; }
            }
            #endregion
            
            #region TranslateXMinControl
            /// <summary>
            /// This property gets or sets the value for 'TranslateXMinControl'.
            /// </summary>
            public ValidationComponent TranslateXMinControl
            {
                get { return translateXMinControl; }
                set { translateXMinControl = value; }
            }
            #endregion
            
            #region TranslateYMaxControl
            /// <summary>
            /// This property gets or sets the value for 'TranslateYMaxControl'.
            /// </summary>
            public ValidationComponent TranslateYMaxControl
            {
                get { return translateYMaxControl; }
                set { translateYMaxControl = value; }
            }
            #endregion
            
            #region TranslateYMinControl
            /// <summary>
            /// This property gets or sets the value for 'TranslateYMinControl'.
            /// </summary>
            public ValidationComponent TranslateYMinControl
            {
                get { return translateYMinControl; }
                set { translateYMinControl = value; }
            }
            #endregion
            
            #region TranslateZMaxControl
            /// <summary>
            /// This property gets or sets the value for 'TranslateZMaxControl'.
            /// </summary>
            public ValidationComponent TranslateZMaxControl
            {
                get { return translateZMaxControl; }
                set { translateZMaxControl = value; }
            }
            #endregion
            
            #region TranslateZMinControl
            /// <summary>
            /// This property gets or sets the value for 'TranslateZMinControl'.
            /// </summary>
            public ValidationComponent TranslateZMinControl
            {
                get { return translateZMinControl; }
                set { translateZMinControl = value; }
            }
            #endregion
            
            #region Worker
            /// <summary>
            /// This property gets or sets the value for 'Worker'.
            /// </summary>
            public BackgroundWorker Worker
            {
                get { return worker; }
                set { worker = value; }
            }
            #endregion
            
        #endregion
        
    }
    #endregion

}
