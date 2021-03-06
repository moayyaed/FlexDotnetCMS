﻿using FrameworkLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.Admin.Controls.Editors
{
    public partial class MediaFieldsEditor : BaseFieldsEditor
    {
        private IMediaDetail mediaDetail;

        public void SetItems(IMediaDetail mediaDetail)
        {
            this.mediaDetail = mediaDetail;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            BindFieldTypeDropDown(FieldTypeDropDown);
            UpdateVisibility();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindItemList();
            //UpdateVisibility();
        }

        private void BindItemList()
        {
            if (mediaDetail == null)
                return;

            ItemList.DataSource = mediaDetail.Fields.OrderBy(i => i.OrderIndex).ToList();
            ItemList.DataBind();
        }

        private void UpdateVisibility()
        {
            var id = long.Parse(FieldID.Value);
            var field = (MediaDetailField)BaseMapper.GetDataModel().Fields.Find(id);

            if (field != null)
            {
                BindVisibility(field);
            }

        }

        private void BindVisibility(MediaDetailField mediaField)
        {
            var possibleMediaTypeField = mediaField?.MediaDetail?.MediaType.Fields.SingleOrDefault(i => i.FieldCode == mediaField.FieldCode);

            if (possibleMediaTypeField != null)
            {
                AssociateWithMediaTypeFieldWrapper.Visible = true;
                AssociateWithMediaTypeField.Checked = (mediaField.MediaTypeField != null);

                if (AssociateWithMediaTypeField.Checked)
                {
                    UseMediaTypeFieldFrontEndLayout.Checked = mediaField.UseMediaTypeFieldFrontEndLayout;
                    UseMediaTypeFieldFrontEndLayoutWrapper.Visible = true;
                    UseMediaTypeFieldDescriptionWrapper.Visible = true;
                }
                else
                {
                    UseMediaTypeFieldFrontEndLayoutWrapper.Visible = false;
                    UseMediaTypeFieldFrontEndLayout.Checked = false;
                    mediaField.UseMediaTypeFieldFrontEndLayout = false;                    
                    UseMediaTypeFieldDescriptionWrapper.Visible = false;
                }
            }
            else
            {
                UseMediaTypeFieldDescriptionWrapper.Visible = UseMediaTypeFieldFrontEndLayoutWrapper.Visible = AssociateWithMediaTypeFieldWrapper.Visible = false;
                UseMediaTypeFieldFrontEndLayout.Checked = false;

                mediaField.MediaTypeField = null;
                mediaField.UseMediaTypeFieldFrontEndLayout = false;
            }
        }

        public AdminBasePage BasePage
        {
            get
            {
                return (AdminBasePage)this.Page;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (FieldID.Value == "0")
            {
                FieldDetailsTitle.Text = "Create a new field";
                Update.Text = "Add Field";
            }
            else
            {
                FieldDetailsTitle.Text = "Currently Editing Field ID: " + FieldID.Value;
                Update.Text = "Save Field";
            }
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            if (mediaDetail.ID == 0)
            {
                BasePage.DisplayErrorMessage("You can only add fields once you have created the page");
                return;
            }

            if (FieldCode.Text == "")
            {
                BasePage.DisplayErrorMessage("'FieldCode' cannot be blank");
                return;
            }

            var fieldId = long.Parse(FieldID.Value);

            MediaDetailField mediaField = null;

            if (fieldId == 0)
            {
                mediaField = new MediaDetailField();
                UpdatedObjectFromFields(mediaField);

                mediaField.OrderIndex = mediaDetail.Fields.Count;
                mediaField.FieldSettings = "";

                mediaDetail.Fields.Add(mediaField);
            }
            else
            {
                mediaField = mediaDetail.Fields.SingleOrDefault(i => i.ID == fieldId);                

                var previouslyAssociateWithMediaTypeField = mediaField.MediaTypeFieldID;

                UpdatedObjectFromFields(mediaField);

                if ((previouslyAssociateWithMediaTypeField == 0 || previouslyAssociateWithMediaTypeField == null) && mediaField.MediaTypeFieldID != 0)
                {
                    mediaField.UseMediaTypeFieldDescription = mediaField.UseMediaTypeFieldFrontEndLayout = true;
                }
            }            

            var returnObj = MediaDetailsMapper.Update(mediaDetail);

            if (!returnObj.IsError)
            {
                FieldID.Value = mediaField.ID.ToString();
                BindVisibility(mediaField);
                BindItemList();

                mediaField.MediaDetail.RemoveFromCache();
            }
            else
            {
                BasePage.DisplayErrorMessage("Error", returnObj.Error);
            }
        }

        private void UpdatedObjectFromFields(MediaDetailField mediaField)
        {
            mediaField.MediaDetailID = mediaDetail.ID;
            mediaField.FieldCode = FieldCode.Text;
            mediaField.FieldLabel = FieldLabel.Text;
            mediaField.AdminControl = AdminControl.Text;
            mediaField.FieldValue = ParserHelper.ParseData(FieldValue.Text, BasePage.TemplateVars, true);
            mediaField.GroupName = GroupName.Text;
            mediaField.RenderLabelAfterControl = RenderLabelAfterControl.Checked;
            mediaField.GetAdminControlValue = GetAdminControlValue.Text;
            mediaField.SetAdminControlValue = SetAdminControlValue.Text;
            mediaField.FrontEndLayout = FrontEndLayout.Text;
            mediaField.FieldDescription = FieldDescription.GetValue().ToString();
            mediaField.UseMediaTypeFieldDescription = UseMediaTypeFieldDescription.Checked;                    
            mediaField.ShowFrontEndFieldEditor = ShowFrontEndFieldEditor.Checked;
            mediaField.IsGlobalField = IsGlobalField.Checked;

            if (string.IsNullOrEmpty(UsageExample.Text))
            {
                if (mediaField.MediaDetail == null && mediaField.MediaDetailID != 0)
                {
                    mediaField.MediaDetail = (MediaDetail)MediaDetailsMapper.GetByID(mediaField.MediaDetailID);
                }

                if (mediaField.MediaDetail != null)
                {
                    UsageExample.Text = "{Field:" + mediaField.FieldCode + "} OR {{Load:" + mediaField.MediaDetail.MediaID + "}.Field:" + mediaField.FieldCode + "}";
                }
            }

            mediaField.UsageExample = UsageExample.Text;            


            if (mediaField.FieldSettings == null)
            {
                mediaField.FieldSettings = "";
            }

            if (mediaField.FrontEndSubmissions == null)
                mediaField.FrontEndSubmissions = "";


            var mediaTypeField = mediaDetail.MediaType.Fields.SingleOrDefault(i => i.FieldCode == mediaField.FieldCode);

            if (mediaTypeField != null)
            {
                mediaField.UseMediaTypeFieldFrontEndLayout = UseMediaTypeFieldFrontEndLayout.Checked;

                if (AssociateWithMediaTypeField.Checked)
                {
                    mediaField.MediaTypeField = mediaTypeField;
                }
                else
                {
                    mediaField.MediaTypeField = null;
                }
            }
            else
            {
                mediaField.UseMediaTypeFieldFrontEndLayout = false;
            }

            mediaField.DateCreated = DateTime.Now;
            mediaField.DateLastModified = DateTime.Now;

            //BindVisibility(mediaField);
        }

        private void UpdatedFieldsFromObject(MediaDetailField mediaField)
        {
            FieldID.Value = mediaField.ID.ToString();
            FieldCode.Text = mediaField.FieldCode;
            FieldLabel.Text = mediaField.FieldLabel;
            AdminControl.Text = mediaField.AdminControl;
            FieldValue.Text = ParserHelper.ParseData(mediaField.FieldValue, BasePage.TemplateVars);
            GroupName.Text = mediaField.GroupName;
            RenderLabelAfterControl.Checked = mediaField.RenderLabelAfterControl;
            FrontEndLayout.Text = mediaField.FrontEndLayout;
            GetAdminControlValue.Text = mediaField.GetAdminControlValue;
            SetAdminControlValue.Text = mediaField.SetAdminControlValue;
            UseMediaTypeFieldFrontEndLayout.Checked = mediaField.UseMediaTypeFieldFrontEndLayout;
            FieldDescription.SetValue(mediaField.FieldDescription);
            UseMediaTypeFieldDescription.Checked = mediaField.UseMediaTypeFieldDescription;            
            ShowFrontEndFieldEditor.Checked = mediaField.ShowFrontEndFieldEditor;
            IsGlobalField.Checked = mediaField.IsGlobalField;
            UsageExample.Text = mediaField.UsageExample;
			
			var jsonObj = JObject.Parse(mediaField.ToJson());
			LoadJson.Text = jsonObj.ToString();

			BindVisibility(mediaField);
        }

        protected void ItemList_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            ItemList.PageIndex = e.NewPageIndex;
            ItemList.DataBind();
        }

        public MediaDetailField GetDataItemFromSender(Control sender)
        {
            //var dataItemIndex = ((ItemList.PageSize * ItemList.PageIndex) +
            var dataItemIndex = ((GridViewRow)(sender).Parent.Parent).DataItemIndex;
            var dataItem = ((List<MediaDetailField>)ItemList.DataSource).ElementAt(dataItemIndex);

            return dataItem;
        }

        protected void Edit_Click(object sender, EventArgs e)
        {
            var field = GetDataItemFromSender((Control)sender);

            if (field != null)
            {
                UpdatedFieldsFromObject(field);
            }
        }

        protected void Delete_Click(object sender, EventArgs e)
        {
            var field = GetDataItemFromSender((Control)sender);

            if (field != null && field.ID !=0)
            {
                var mediaDetailFields = BaseMapper.GetDataModel().Fields.Where(i => i.ID == field.ID);

                foreach (MediaDetailField mediaDetailField in mediaDetailFields)
                {
                    var fieldAssociations = mediaDetailField.FieldAssociations.ToList();

                    foreach (var fieldAssociation in fieldAssociations)
                    {
                        BaseMapper.GetDataModel().FieldAssociations.Remove(fieldAssociation);
                    }

                    BaseMapper.DeleteObjectFromContext(mediaDetailField);
                }

                var returnObj = MediaDetailsMapper.Update(mediaDetail);

                if (!returnObj.IsError)
                {
                    UpdatedFieldsFromObject(new MediaDetailField());
                    BindItemList();
                }
                else
                {
                    BasePage.DisplayErrorMessage("Error", returnObj.Error);
                }
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            UpdatedFieldsFromObject(new MediaDetailField());
        }

        protected void ItemList_DataBound(object sender, EventArgs e)
        {
            ItemList.UseAccessibleHeader = true;
            if (ItemList.HeaderRow != null)
            {
                ItemList.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

		protected void LoadFromJson_Click(object sender, EventArgs e)
		{
			var text = LoadJson.Text.Trim();

			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					var newField = StringHelper.JsonToObject<MediaDetailField>(text);

					newField.ID = 0;
					newField.UsageExample = "";

					UpdatedFieldsFromObject(newField);

					BasePage.DisplaySuccessMessage("Successfully loaded from JSON");
				}
				catch (Exception ex)
				{
					BasePage.DisplayErrorMessage("Error loading JSON: "+ ex.Message);
				}
			}
		}

		protected void ExportFields_Click(object sender, EventArgs e)
		{
			if (mediaDetail != null)
			{
				var json = mediaDetail.Fields.ToJson(2);
				var jsonObj = JObject.Parse(json);

				ExportImportFieldsJson.Text = jsonObj["Comparer"].ToString();
				ExportImportFieldsPanel.Visible = true;
			}
		}

		protected void ImportFields_Click(object sender, EventArgs e)
		{
			var importFields = StringHelper.JsonToObject<List<MediaDetailField>>(ExportImportFieldsJson.Text);

			foreach (var field in importFields)
			{
				var foundField = mediaDetail.Fields.FirstOrDefault(i => i.FieldCode == field.FieldCode);

				if (foundField != null)
					continue;

				field.ID = 0;
				field.DateCreated = field.DateLastModified = DateTime.Now;

				if (field.MediaTypeField?.MediaTypeID != mediaDetail.MediaTypeID)
				{
					field.UseMediaTypeFieldFrontEndLayout = false;
					field.UseMediaTypeFieldDescription = false;
					field.MediaTypeField = null;
					field.MediaTypeFieldID = null;

					var foundMediaTypeField = mediaDetail.MediaType.Fields.FirstOrDefault(i => i.FieldCode == field.FieldCode);

					if (foundMediaTypeField != null)
					{
						field.MediaTypeFieldID = foundMediaTypeField.ID;
					}
				}

				mediaDetail.Fields.Add(field);
			}

			var returnObj = MediaDetailsMapper.Update(mediaDetail);

			if (returnObj.IsError)
			{
				BasePage.DisplayErrorMessage("Error", returnObj.Error);
			}
			else
			{
				BasePage.DisplaySuccessMessage("Successfully imported fields");
			}

			BindItemList();
		}

	}
}