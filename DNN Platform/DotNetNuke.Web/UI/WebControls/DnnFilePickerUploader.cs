﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Localization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Common;

namespace DotNetNuke.Web.UI.WebControls
{
    public class DnnFilePickerUploader: UserControl
	{
		#region Private Fields
        private const string MyFileName = "filepickeruploader.ascx";
	    private int? _portalId = null;

		#endregion

		#region protected properties

        protected DnnFileDropDownList FilesComboBox;
        protected DnnFolderDropDownList FoldersComboBox;

        protected string FolderLabel
        {
            get
            {
                return Localization.GetString("Folder", Localization.GetResourceFile(this, MyFileName));
            }
        }

        protected string FileLabel
        {
            get
            {
                return Localization.GetString("File", Localization.GetResourceFile(this, MyFileName));
            }
        }

        protected string UploadFileLabel
        {
            get
            {
                return Localization.GetString("UploadFile", Localization.GetResourceFile(this, MyFileName));
            }
        }

        protected string DropFileLabel
        {
            get
            {
                return Localization.GetString("DropFile", Localization.GetResourceFile(this, MyFileName));
            }
        }
        
		#endregion

		#region Public Properties

		public bool UsePersonalFolder { get; set; }
        public string FilePath
        {
            get 
            {
                EnsureChildControls();

                var path = string.Empty;
                if (FoldersComboBox.SelectedFolder != null && FilesComboBox.SelectedFile != null)
                {
                    path = FilesComboBox.SelectedFile.RelativePath;
                }

                return path;
            }

            set
            {
                EnsureChildControls();
                if (!string.IsNullOrEmpty(value))
                {
                    var file = FileManager.Instance.GetFile(PortalId, value);
                    if (file != null)
                    {
                        FoldersComboBox.SelectedFolder = FolderManager.Instance.GetFolder(file.FolderId);
                        FilesComboBox.SelectedFile = file;
                    }
                }
            }
        }
        public int FileID
        {
            get
            {
                EnsureChildControls();
                
                return FilesComboBox.SelectedFile != null ? FilesComboBox.SelectedFile.FileId : Null.NullInteger;
            }

            set
            {
                EnsureChildControls();
                var file = FileManager.Instance.GetFile(value);
                if (file != null)
                {
                    FoldersComboBox.SelectedFolder = FolderManager.Instance.GetFolder(file.FolderId);
                    FilesComboBox.SelectedFile = file;
                }
            }
        }


        public string FolderPath 
        { 
            get { return FoldersComboBox.SelectedFolder != null ? FoldersComboBox.SelectedFolder.FolderPath : string.Empty; }
        }

        public string FileFilter { get; set; }
        public bool Required { get; set; }
        public UserInfo User { get; set; }

	    public int PortalId
	    {
		    get
		    {
			    return !_portalId.HasValue ? PortalSettings.Current.PortalId : _portalId.Value;
		    }
			set
			{
				_portalId = value;
			}
	    }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            FoldersComboBox.SelectItemDefaultText = "Site Root";
            FoldersComboBox.OnClientSelectionChanged.Add("dnn.dnnFileUpload.Folders_Changed");
            FoldersComboBox.Options.Services.Parameters.Add("permission", "READ,ADD");

            FilesComboBox.OnClientSelectionChanged.Add("dnn.dnnFileUpload.Files_Changed");
            FilesComboBox.SelectItemDefaultText = "<" + Localization.GetString("None_Specified") + ">";
            FilesComboBox.Services.Parameters.Add("filter", FileFilter);

            LoadFolders();
            jQuery.RegisterFileUpload(Page);
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        }

        private void LoadFolders()
        {
            if (UsePersonalFolder)
            {
                var user = User ?? UserController.GetCurrentUserInfo();
                var userFolder = FolderManager.Instance.GetUserFolder(user);
                FoldersComboBox.SelectedItem = new ListItem
                                                   {
                                                       Text = FolderManager.Instance.MyFolderName, 
                                                       Value = userFolder.FolderID.ToString(CultureInfo.InvariantCulture)
                                                   };
                FoldersComboBox.Enabled = false;
            }
            else
            {
                //select folder
                string fileName;
                string folderPath;
                if (!string.IsNullOrEmpty(FilePath))
                {
                    fileName = FilePath.Substring(FilePath.LastIndexOf("/") + 1);
                    folderPath = string.IsNullOrEmpty(fileName) ? FilePath : FilePath.Replace(fileName, "");
                }
                else
                {
                    fileName = FilePath;
                    folderPath = string.Empty;
                }

                FoldersComboBox.SelectedFolder = FolderManager.Instance.GetFolder(PortalId, folderPath);

                if (!string.IsNullOrEmpty(fileName))
                {
                    FilesComboBox.SelectedFile = FileManager.Instance.GetFile(FoldersComboBox.SelectedFolder, fileName);
                }
            }
        }
    }
}
