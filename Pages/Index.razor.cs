

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
        private ValidationComponent enableWindCheckBox;
        private ValidationComponent forceXMinControl;
        private ValidationComponent forceXMaxControl;        
        private ValidationComponent forceZMinControl;
        private ValidationComponent forceZMaxControl;
        private ValidationComponent velocityXMinControl;
        private ValidationComponent velocityXMaxControl;        
        private ValidationComponent velocityZMinControl;
        private ValidationComponent velocityZMaxControl;
        private ValidationComponent forcePercentControl;
        private List<ValidationComponent> windControls;
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
        private int progressIncrement;
        private string labelColor;
        private bool enableWind;
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

                    // if the value for HasForceXMaxControl is true
                    if (HasForceXMaxControl)
                    {
                        // Set the initial value
                        ForceXMaxControl.SetTextValue(settings.ForceXMax.ToString());
                    }

                    // if the value for HasForceXMinControl is true
                    if (HasForceXMinControl)
                    {
                        // Set the initial value
                        ForceXMinControl.SetTextValue(settings.ForceXMin.ToString());
                    }

                    // if the value for HasForceZMaxControl is true
                    if (HasForceZMaxControl)
                    {
                        // Set the initial value
                        ForceZMaxControl.SetTextValue(settings.ForceZMax.ToString());
                    }

                    // if the value for HasForceZMinControl is true
                    if (HasForceZMinControl)
                    {
                        // Set the initial value
                        ForceZMinControl.SetTextValue(settings.ForceZMin.ToString());
                    }

                    // if the value for HasVelocityXMaxControl is true
                    if (HasVelocityXMaxControl)
                    {
                        // Set the initial value
                        VelocityXMaxControl.SetTextValue(settings.VelocityXMax.ToString());
                    }

                    // if the value for HasVelocityXMinControl is true
                    if (HasVelocityXMinControl)
                    {
                        // Set the initial value
                        VelocityXMinControl.SetTextValue(settings.VelocityXMin.ToString());
                    }

                    // if the value for HasVelocityZMaxControl is true
                    if (HasVelocityZMaxControl)
                    {
                        // Set the initial value
                        VelocityZMaxControl.SetTextValue(settings.VelocityZMax.ToString());
                    }

                    // if the value for HasVelocityZMinControl is true
                    if (HasVelocityZMinControl)
                    {
                        // Set the initial value
                        VelocityZMinControl.SetTextValue(settings.VelocityZMin.ToString());
                    }

                    // if the value for HasForcePercentControl is true
                    if (HasForcePercentControl)
                    {
                        // set the value for ForcePercent
                        ForcePercentControl.SetTextValue(settings.ForcePercent.ToString());
                    }

                    // Disable the WinddControls at startup
                    RefreshWindControls();
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
                    RandomizationResult result = await Randomizer.RandomizeAsync(settings, Callback);

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

                            // Setup the label
                            StatusLabel.SetTextValue("Zipping file, please wait...");
                            StatusLabel.Refresh();

                            // if the Invisible Sprite exists
                            if (HasInvisibleSprite)
                            {
                                // Start the timer
                                invisibleSprite.Start();
                            }

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
                            DownloadPath = "../Downloads/SnowScenes/" + fileInfo2.Name;

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

                        // if the value for HasInvisibleSprite is true
                        if (HasInvisibleSprite)
                        {
                            InvisibleSprite.Stop();
                        }
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

                    // Set hte progress increment
                    SetProgressIncrement(Settings.ObjectsToCreate);
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

                // Store the value
                settings.EnableForce = EnableWind;

                // if the value for EnableWind is true
                if (EnableWind)
                {
                    // if the value for HasForcePercentControl is true
                    if (HasForcePercentControl)
                    {
                        // Set the ForcePercent
                        settings.ForcePercent = NumericHelper.ParseInteger(ForcePercentControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasForceXMaxControl)
                    {
                        // Set the value
                        settings.ForceXMax = NumericHelper.ParseInteger(ForceXMaxControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasForceXMinControl)
                    {
                        // Set the value
                        settings.ForceXMin = NumericHelper.ParseInteger(ForceXMinControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasForceZMaxControl)
                    {
                        // Set the value
                        settings.ForceZMax = NumericHelper.ParseInteger(ForceZMaxControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasForceZMinControl)
                    {
                        // Set the value
                        settings.ForceZMin = NumericHelper.ParseInteger(ForceZMinControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasVelocityXMaxControl)
                    {
                        // Set the value
                        settings.VelocityXMax = NumericHelper.ParseInteger(VelocityXMaxControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasVelocityXMinControl)
                    {
                        // Set the value
                        settings.VelocityXMin = NumericHelper.ParseInteger(VelocityXMinControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasVelocityZMaxControl)
                    {
                        // Set the value
                        settings.VelocityZMax = NumericHelper.ParseInteger(VelocityZMaxControl.Text, 0, 0);
                    }

                    // if the Control exists
                    if (HasVelocityZMinControl)
                    {
                        // Set the value
                        settings.VelocityZMin = NumericHelper.ParseInteger(VelocityZMinControl.Text, 0, 0);
                    }                
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

                    //// if the ProgressBar
                    //if (HasInvisibleSprite)
                    //{
                    //    // Start the Timer
                    //    InvisibleSprite.Start();
                    //}

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
            
            #region Callback(int progressPercent, bool complete)
            /// <summary>
            /// Callback
            /// </summary>
            public void Callback(int progressPercent, bool complete)
            {
                // if finished
                if (complete)
                {
                    // finished
                    // ShowProgress = false;
                }
                else
                {
                    // Set the percent
                    double percent = (progressPercent / 2);

                    // if greater than 50
                    if (percent >= 50)
                    {
                        // go no higher than 50 here
                        percent = 50;
                    }

                    // Update the value for Percent
                    Percent = (int) percent;
                }

                // Update
                Refresh();
            }
            #endregion
            
            #region RefreshWindControls()
            /// <summary>
            /// Enable or Disable the Wind Controls after Enable Wind is checked or unchecked.
            /// </summary>
            public void RefreshWindControls()
            {
                // If the WindControls collection exists and has one or more items
                if (ListHelper.HasOneOrMoreItems(WindControls))
                {
                    // Iterate the collection of ValidationComponent objects
                    foreach (ValidationComponent component in WindControls)
                    {
                        // if EnableWind is true
                        if (EnableWind)
                        {
                            // Use black
                            component.SetLabelColor("black");
                        }
                        else
                        {
                            // Use gray
                            component.SetLabelColor("gray");
                        }

                        // Update the value for Enabled
                        component.SetEnabled(EnableWind);

                        // Update
                        component.Refresh();
                    }
                }
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
                WindControls = new List<ValidationComponent>();

                // Default Force Values
                settings.ForcePercent = 50;
                settings.ForceXMin = 0;
                settings.ForceXMax = 50;
                settings.ForceYMin = 0;
                settings.ForceYMax = 0;
                settings.ForceZMin = 0;
                settings.ForceZMax = 50;
                settings.VelocityXMin = 0;
                settings.VelocityXMax = 100;
                settings.VelocityYMin = 0;
                settings.VelocityYMax = 0;
                settings.VelocityZMin = 0;
                settings.VelocityZMax = 100;

                // Default to Black
                LabelColor = "black";
            }
            #endregion
            
            #region ReceiveData(Message message)
            /// <summary>
            /// method Receive Data
            /// </summary>
            public void ReceiveData(Message message)
            {
                // If the message object exists
                if (NullHelper.Exists(message))
                {
                    // if the message is from the EnableWindCheckBox
                    if ((HasEnableWindCheckBox) && (TextHelper.IsEqual(message.Sender.Name, EnableWindCheckBox.Name)))
                    {
                        // Set the value for EnableWind
                        EnableWind = EnableWindCheckBox.CheckBoxValue;

                        // Enable the Wind Controls
                        RefreshWindControls();
                    }
                }
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
                    // increment by 1 - 5 based on how many objects to create there are
                    Percent += ProgressIncrement;

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

                        case "EnableWindCheckBox":

                            // Store the control
                            EnableWindCheckBox = component as ValidationComponent;

                            // required
                            break;

                        case "ForceXMin":

                            // Store the control
                            ForceXMinControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(ForceXMinControl);

                            // required
                            break;

                        case "ForceXMax":

                            // Store the control
                            ForceXMaxControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(ForceXMaxControl);

                            // required
                            break;

                        case "ForceZMin":

                            // Store the control
                            ForceZMinControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(ForceZMinControl);

                            // required
                            break;

                        case "ForceZMax":

                            // Store the control
                            ForceZMaxControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(ForceZMaxControl);

                            // required
                            break;

                        case "VelocityXMin":

                            // Store the control
                            VelocityXMinControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(VelocityXMinControl);

                            // required
                            break;

                        case "VelocityXMax":

                            // Store the control
                            VelocityXMaxControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(VelocityXMaxControl);

                            // required
                            break;

                        case "VelocityZMin":

                            // Store the control
                            VelocityZMinControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(VelocityZMinControl);

                            // required
                            break;

                        case "VelocityZMax":

                            // Store the control
                            VelocityZMaxControl = component as ValidationComponent;

                            // Add this control to WindControls
                            WindControls.Add(VelocityZMaxControl);

                            // required
                            break;

                        case "ForcePercent":

                            // Store the control
                            ForcePercentControl = component as ValidationComponent;

                             // Add this control to WindControls
                            WindControls.Add(ForcePercentControl);

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
            
            #region SetProgressIncrement(int objectsToCreate)
            /// <summary>
            /// Set Progress Increment
            /// </summary>
            public void SetProgressIncrement(int objectsToCreate)
            {
                if (objectsToCreate > 5000)
                {
                    // Set to 1
                    ProgressIncrement = 2;
                }
                else if (objectsToCreate >= 4000)
                {
                    // Set to 2
                    ProgressIncrement = 3;
                }                
                else if (objectsToCreate > 2000)
                {
                    // Set to 4
                    ProgressIncrement = 4;
                }
                else
                {
                    // Set to 5
                    ProgressIncrement = 5;
                }
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
           
            #region EnableWind
            /// <summary>
            /// This property gets or sets the value for 'EnableWind'.
            /// </summary>
            public bool EnableWind
            {
                get { return enableWind; }
                set { enableWind = value; }
            }
            #endregion
            
            #region EnableWindCheckBox
            /// <summary>
            /// This property gets or sets the value for 'EnableWindCheckBox'.
            /// </summary>
            public ValidationComponent EnableWindCheckBox
            {
                get { return enableWindCheckBox; }
                set { enableWindCheckBox = value; }
            }
            #endregion
            
            #region ForcePercentControl
            /// <summary>
            /// This property gets or sets the value for 'ForcePercentControl'.
            /// </summary>
            public ValidationComponent ForcePercentControl
            {
                get { return forcePercentControl; }
                set { forcePercentControl = value; }
            }
            #endregion
            
            #region ForceXMaxControl
            /// <summary>
            /// This property gets or sets the value for 'ForceXMaxControl'.
            /// </summary>
            public ValidationComponent ForceXMaxControl
            {
                get { return forceXMaxControl; }
                set { forceXMaxControl = value; }
            }
            #endregion
            
            #region ForceXMinControl
            /// <summary>
            /// This property gets or sets the value for 'ForceXMinControl'.
            /// </summary>
            public ValidationComponent ForceXMinControl
            {
                get { return forceXMinControl; }
                set { forceXMinControl = value; }
            }
            #endregion
            
            #region ForceZMaxControl
            /// <summary>
            /// This property gets or sets the value for 'ForceZMaxControl'.
            /// </summary>
            public ValidationComponent ForceZMaxControl
            {
                get { return forceZMaxControl; }
                set { forceZMaxControl = value; }
            }
            #endregion
            
            #region ForceZMinControl
            /// <summary>
            /// This property gets or sets the value for 'ForceZMinControl'.
            /// </summary>
            public ValidationComponent ForceZMinControl
            {
                get { return forceZMinControl; }
                set { forceZMinControl = value; }
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
            
            #region HasEnableWindCheckBox
            /// <summary>
            /// This property returns true if this object has an 'EnableWindCheckBox'.
            /// </summary>
            public bool HasEnableWindCheckBox
            {
                get
                {
                    // initial value
                    bool hasEnableWindCheckBox = (this.EnableWindCheckBox != null);
                    
                    // return value
                    return hasEnableWindCheckBox;
                }
            }
            #endregion
            
            #region HasForcePercentControl
            /// <summary>
            /// This property returns true if this object has a 'ForcePercentControl'.
            /// </summary>
            public bool HasForcePercentControl
            {
                get
                {
                    // initial value
                    bool hasForcePercentControl = (this.ForcePercentControl != null);
                    
                    // return value
                    return hasForcePercentControl;
                }
            }
            #endregion
            
            #region HasForceXMaxControl
            /// <summary>
            /// This property returns true if this object has a 'ForceXMaxControl'.
            /// </summary>
            public bool HasForceXMaxControl
            {
                get
                {
                    // initial value
                    bool hasForceXMaxControl = (this.ForceXMaxControl != null);
                    
                    // return value
                    return hasForceXMaxControl;
                }
            }
            #endregion
            
            #region HasForceXMinControl
            /// <summary>
            /// This property returns true if this object has a 'ForceXMinControl'.
            /// </summary>
            public bool HasForceXMinControl
            {
                get
                {
                    // initial value
                    bool hasForceXMinControl = (this.ForceXMinControl != null);
                    
                    // return value
                    return hasForceXMinControl;
                }
            }
            #endregion
            
            #region HasForceZMaxControl
            /// <summary>
            /// This property returns true if this object has a 'ForceZMaxControl'.
            /// </summary>
            public bool HasForceZMaxControl
            {
                get
                {
                    // initial value
                    bool hasForceZMaxControl = (this.ForceZMaxControl != null);
                    
                    // return value
                    return hasForceZMaxControl;
                }
            }
            #endregion
            
            #region HasForceZMinControl
            /// <summary>
            /// This property returns true if this object has a 'ForceZMinControl'.
            /// </summary>
            public bool HasForceZMinControl
            {
                get
                {
                    // initial value
                    bool hasForceZMinControl = (this.ForceZMinControl != null);
                    
                    // return value
                    return hasForceZMinControl;
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
            
            #region HasVelocityXMaxControl
            /// <summary>
            /// This property returns true if this object has a 'VelocityXMaxControl'.
            /// </summary>
            public bool HasVelocityXMaxControl
            {
                get
                {
                    // initial value
                    bool hasVelocityXMaxControl = (this.VelocityXMaxControl != null);
                    
                    // return value
                    return hasVelocityXMaxControl;
                }
            }
            #endregion
            
            #region HasVelocityXMinControl
            /// <summary>
            /// This property returns true if this object has a 'VelocityXMinControl'.
            /// </summary>
            public bool HasVelocityXMinControl
            {
                get
                {
                    // initial value
                    bool hasVelocityXMinControl = (this.VelocityXMinControl != null);
                    
                    // return value
                    return hasVelocityXMinControl;
                }
            }
            #endregion
            
            #region HasVelocityZMaxControl
            /// <summary>
            /// This property returns true if this object has a 'VelocityZMaxControl'.
            /// </summary>
            public bool HasVelocityZMaxControl
            {
                get
                {
                    // initial value
                    bool hasVelocityZMaxControl = (this.VelocityZMaxControl != null);
                    
                    // return value
                    return hasVelocityZMaxControl;
                }
            }
            #endregion
            
            #region HasVelocityZMinControl
            /// <summary>
            /// This property returns true if this object has a 'VelocityZMinControl'.
            /// </summary>
            public bool HasVelocityZMinControl
            {
                get
                {
                    // initial value
                    bool hasVelocityZMinControl = (this.VelocityZMinControl != null);
                    
                    // return value
                    return hasVelocityZMinControl;
                }
            }
            #endregion
            
            #region HasWindControls
            /// <summary>
            /// This property returns true if this object has a 'WindControls'.
            /// </summary>
            public bool HasWindControls
            {
                get
                {
                    // initial value
                    bool hasWindControls = (this.WindControls != null);
                    
                    // return value
                    return hasWindControls;
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
            
            #region ProgressIncrement
            /// <summary>
            /// This property gets or sets the value for 'ProgressIncrement'.
            /// </summary>
            public int ProgressIncrement
            {
                get { return progressIncrement; }
                set { progressIncrement = value; }
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
            
            #region VelocityXMaxControl
            /// <summary>
            /// This property gets or sets the value for 'VelocityXMaxControl'.
            /// </summary>
            public ValidationComponent VelocityXMaxControl
            {
                get { return velocityXMaxControl; }
                set { velocityXMaxControl = value; }
            }
            #endregion
            
            #region VelocityXMinControl
            /// <summary>
            /// This property gets or sets the value for 'VelocityXMinControl'.
            /// </summary>
            public ValidationComponent VelocityXMinControl
            {
                get { return velocityXMinControl; }
                set { velocityXMinControl = value; }
            }
            #endregion
            
            #region VelocityZMaxControl
            /// <summary>
            /// This property gets or sets the value for 'VelocityZMaxControl'.
            /// </summary>
            public ValidationComponent VelocityZMaxControl
            {
                get { return velocityZMaxControl; }
                set { velocityZMaxControl = value; }
            }
            #endregion
            
            #region VelocityZMinControl
            /// <summary>
            /// This property gets or sets the value for 'VelocityZMinControl'.
            /// </summary>
            public ValidationComponent VelocityZMinControl
            {
                get { return velocityZMinControl; }
                set { velocityZMinControl = value; }
            }
            #endregion
            
            #region WindControls
            /// <summary>
            /// This property gets or sets the value for 'WindControls'.
            /// </summary>
            public List<ValidationComponent> WindControls
            {
                get { return windControls; }
                set { windControls = value; }
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
