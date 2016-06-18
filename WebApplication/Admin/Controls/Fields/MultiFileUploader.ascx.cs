﻿using FrameworkLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.Admin.Controls.Fields
{
    public partial class MultiFileUploader : BaseFieldControl
    {
        private bool hasRun = false;

        public override void RenderControlInAdmin()
        {
            base.RenderControlInAdmin();
        }

        public override void RenderControlInFrontEnd()
        {
            base.RenderControlInFrontEnd();
        }

        public override object GetValue()
        {
            var field = GetField();

            return field.FieldAssociations.OrderBy(i => i.OrderIndex);
        }

        public DirectoryInfo GetFolderPath()
        {
            var field = GetField();

            var folderPath = SaveToFolder;

            if (folderPath == "")
                folderPath = "~/media/uploads/";

            return new DirectoryInfo(FrameworkLibrary.URIHelper.ConvertToAbsPath(folderPath + "fields/" + field.ID + "/"));
        }

        public override void SetValue(object value)
        {
            var field = GetField();
            var hasDeleted = false;
            var hasReordered = false;

            if (IsPostBack && MultiFileUpload.HasFiles && !hasRun)
            {
                hasRun = true;

                var index = 0;
                foreach (var file in MultiFileUpload.PostedFiles)
                {
                    var fileInfo = (new System.IO.FileInfo(GetFolderPath() + file.FileName));

                    if (!fileInfo.Directory.Exists)
                        fileInfo.Directory.Create();

                    if (fileInfo.Directory.Exists)
                    {
                        file.SaveAs(fileInfo.FullName);

                        var filePath = URIHelper.ConvertAbsUrlToTilda(URIHelper.ConvertAbsPathToAbsUrl(fileInfo.FullName));

                        if (!field.FieldAssociations.Any(i => i.MediaDetail.PathToFile == filePath))
                        {
                            var fieldAssociation = new FieldAssociation();
                            fieldAssociation.MediaDetail = (MediaDetail)PagesMapper.CreateObject(MediaTypesMapper.GetByEnum(MediaTypeEnum.Page).ID, MediasMapper.CreateObject(), AdminBasePage.SelectedMedia);
                            fieldAssociation.MediaDetail.PathToFile = filePath;
                            fieldAssociation.MediaDetail.PublishDate = DateTime.Now;
                            fieldAssociation.MediaDetail.CreatedByUser = fieldAssociation.MediaDetail.LastUpdatedByUser = FrameworkSettings.CurrentUser;
                            fieldAssociation.MediaDetail.CachedVirtualPath = fieldAssociation.MediaDetail.CalculatedVirtualPath();
                            fieldAssociation.MediaDetail.Language = SettingsMapper.GetSettings().DefaultLanguage;
                            fieldAssociation.MediaDetail.ShowInSiteTree = false;

                            field.FieldAssociations.Add(fieldAssociation);

                            index++;
                        }
                    }
                }
            }

            if (FilesToDelete.Text != "" && FilesToDelete.Text != "[]")
            {
                var deleteIds = StringHelper.JsonToObject<List<long>>(FilesToDelete.Text);

                foreach (var id in deleteIds)
                {
                    var fieldAssociation = field.FieldAssociations.SingleOrDefault(i => i.ID == id);

                    if (fieldAssociation != null)
                    {
                        /*var absPathToFile = URIHelper.ConvertToAbsPath(fieldFile.PathToFile);

                        if (File.Exists(absPathToFile))
                        {
                            File.Delete(absPathToFile);
                        }*/

                        var returnObj = MediaDetailsMapper.DeletePermanently(fieldAssociation.MediaDetail);

                        if (!returnObj.IsError)
                            BaseMapper.DeleteObjectFromContext(fieldAssociation);

                        //field.FieldFiles.Remove(fieldFile);

                        hasDeleted = true;
                    }
                }
            }

            if (ReorderFiles.Text != "" && ReorderFiles.Text != "[]")
            {
                var reorderedIds = StringHelper.JsonToObject<List<long>>(ReorderFiles.Text);

                var index = 0;
                foreach (var id in reorderedIds)
                {
                    if (id == null)
                        return;

                    var fieldAssociation = field.FieldAssociations.SingleOrDefault(i => i.ID == id);

                    if (fieldAssociation != null)
                    {
                        fieldAssociation.OrderIndex = index;
                        hasReordered = true;

                        index++;
                    }
                }
            }

            if (hasDeleted || hasReordered)
                BaseMapper.SaveDataModel();


            BindValues();
        }

        private void BindValues()
        {
            Values.DataSource = GetValue();
            Values.DataBind();

            FilesToDelete.Text = "[]";
            ReorderFiles.Text = "[]";
        }

        public string SaveToFolder { get; set; }


        protected void AddFieldFile_Click(object sender, EventArgs e)
        {
            var field = GetField();

            var fieldAssociation = new FieldAssociation();
            fieldAssociation.MediaDetail = (MediaDetail)PagesMapper.CreateObject(MediaTypesMapper.GetByEnum(MediaTypeEnum.Page).ID, MediasMapper.CreateObject(), AdminBasePage.SelectedMedia);
            fieldAssociation.MediaDetail.PathToFile = "/media/images/icons/File.jpg";
            fieldAssociation.MediaDetail.PublishDate = DateTime.Now;
            fieldAssociation.MediaDetail.CreatedByUser = fieldAssociation.MediaDetail.LastUpdatedByUser = FrameworkSettings.CurrentUser;
            fieldAssociation.MediaDetail.CachedVirtualPath = fieldAssociation.MediaDetail.CalculatedVirtualPath();
            fieldAssociation.MediaDetail.Language = SettingsMapper.GetSettings().DefaultLanguage;
            fieldAssociation.MediaDetail.ShowInSiteTree = false;

            field.FieldAssociations.Add(fieldAssociation);
            var returnObj = BaseMapper.SaveDataModel();

            BindValues();
        }
    }
}