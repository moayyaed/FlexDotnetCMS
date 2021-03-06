﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoggedInHeader.ascx.cs" Inherits="WebApplication.Controls.OnLogin.LoggedInHeader" %>
<%@ Register Src="~/Controls/LoggedIn/VisualLayoutEditor.ascx" TagPrefix="Admin" TagName="VisualLayoutEditor" %>

<% if (BasePage.CurrentUser != null && BasePage.CurrentUser.HasPermission(PermissionsEnum.FrontEndEditor))
    { %>

<asp:Panel runat="server" ID="LoggedInHeaderPanel" ClientIDMode="Static">      
	<script>
		var popCloseRefreshPage = true;
	</script>
    <Admin:VisualLayoutEditor runat="server" id="VisualLayoutEditor" Visible="false"/>

    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />
    <script src="//cdnjs.cloudflare.com/ajax/libs/jquery-cookie/1.4.1/jquery.cookie.min.js"></script>

    <a id="SlideTab"><i class="fa fa-arrow-down" aria-hidden="true"></i>CMS Shortcuts</a>
    <asp:Panel ID="AccessCMSPermissionsPanel" runat="server" Visible="true" ClientIDMode="Static">
        <div>                        
            <a ID="QuickEditCurrentPage" class="colorbox iframe button" data-OnColorboxClose="RefreshPage()" href="<%= CurrentMediaDetailAdminUrl %>&masterFilePath=~/Admin/Views/MasterPages/Popup.Master"><i class="fa fa-pencil"></i>&nbsp;Edit Page</a>                
            <div id="AdminPanel" runat="server">
                <% if (CanAccessVisualLayoutEditor)
                    {
                %>
                <a ID="ToggleVisualEditor" class="button" href="javascript:void(0)" onclick="ToggleVisualEditor()"><i class="fa fa-wrench"></i>&nbsp;Toggle Visual Layout Editor</a>
                <%  } %>
                <!--<a ID="EdiSettings" class="colorbox iframe button" data-OnColorboxClose="RefreshPage()" href="<%= URIHelper.BaseUrl %>Admin/Views/PageHandlers/Settings/Default.aspx?masterFilePath=~/Admin/Views/MasterPages/Popup.Master"><i class="fa fa-wrench"></i>&nbsp;Edit Settings</a>
                <a ID="EditTemplate" class="colorbox iframe button" data-OnColorboxClose="RefreshPage()" href="<%= URIHelper.BaseUrl %>Admin/Views/PageHandlers/MasterPages/Detail.aspx?id=<%= BasePage.CurrentMediaDetail.GetMasterPage().ID %>"><i class="fa fa-file-code-o"></i>&nbsp;Edit Template</a>
                <a ID="EditMediaType" class="colorbox iframe button" data-OnColorboxClose="RefreshPage()" href="<%= URIHelper.BaseUrl %>Admin/Views/PageHandlers/MediaTypes/Detail.aspx?id=<%= BasePage.CurrentMediaDetail.MediaTypeID %>"><i class="fa fa-file-code-o"></i>&nbsp;Edit Media Type</a>-->
            </div>
        </div>
        <div> 
            <a ID="EditCurrentPage" Target="_blank" class="button" data-OnColorboxClose="RefreshPage()" href="<%= CurrentMediaDetailAdminUrl %>"><i class="fa fa-external-link"></i>&nbsp;Edit in CMS</a>
        </div>
        <div class="clear"></div>
    </asp:Panel>
    <style>
        #SlideTab {
            background-color: red;
            display:block;
            cursor: pointer;         
            text-align: center;
            padding: 5px;
            color: #fff;
        }

            #SlideTab .fa {
                margin-right: 5px;
            }

        #LoggedInHeaderPanel {
            background-color: #000;
            color: #fff;
            position: fixed;
            bottom:0;
            right:0;
            z-index:999999;
            display:none;
        }

        #LoggedInHeaderPanel a.button{
            display: block;
            color: #fff;                
            border: 1px solid #fff;
            padding: 5px;              
            margin: 10px;
            text-decoration: none;
        }

        #LoggedInHeaderPanel a.button:hover{
            background-color: red;
        }

        #LoggedInHeaderPanel:after {
            clear: both;
        }

        div.clear {
            clear: both;
        }

        .floatLeft {
            float: left;
        }

        .floatRight {
            float: right;
        }

        .field {        
            position: relative;
            /*padding-top: 25px;*/
        }

            .field.hide {
                padding:0;
                margin:0;
                border:none !important;                

            }
            .field.hide .fieldControls{
                display:none !important;
            }

            .field:not(.hide).hover, .field:not(.hide):hover {
                outline: 2px dotted;
				background-color: rgba(1,1,1,0.1);
            }

            .field .fieldControls a:hover {				
                opacity: 1;
				color: red;
				/*background-color: red;*/
            }
            .field .fieldControls a {				
				color: #000;
				font-weight: bold;
				font-size: 12px;
				/*background-color: #000;*/
				padding: 2px;
				opacity: 1;
            }
                .field .fieldControls a.remove {
                    display:none;
					margin-left: 10px;
                }

            .field .fieldControls.show {
                display: block;				
				text-align: left;
				margin-left:5px;
            }

    		.col > .field  .fieldControls.show {				
    		}

            .field .field .fieldControls {		
				float: none;
            }

    </style>

    <script type="text/javascript">

        ajaxOptions.onLoad = function (el) {
            CreateFieldsEditor();
        }

        function RefreshPage()
		{            
			//if (popCloseRefreshPage) {
				window.location.href = window.location.href;
			//}
        }

        function ToggleVisualEditor()
        {            
            if (window.location.search.toLowerCase().indexOf("visuallayouteditor=true") == -1)
            {
                var url = window.location.href;

                if (url.indexOf("?") == -1)
                {
                    url = url + "?";
                }
                else
                {
                    url = url + "&";
                }

                url = url + "VisualLayoutEditor=true";

                window.location.href = url;
            }
            else
            {
                window.location.href = window.location.href.toLowerCase().replace("&visuallayouteditor=true", "").replace("?visuallayouteditor=true", "");
            }
        }

        function HideFieldsEditor() {
            $(".field").addClass("hide");
            $(".field .fieldControls").removeClass("show");           
        }

        function ShowFieldsEditor() {
            $(".field").removeClass("hide");
            $(".field .fieldControls").addClass("show");
        }

		function CreateFieldsEditor() {
			$("[data-fieldid] .fieldControls").remove();

            $("[data-fieldid]").each(function () {
                var fieldId = $(this).attr("data-fieldid");
                var fieldcode = $(this).attr("data-fieldcode");
				var mediaId = $(this).attr("data-mediaid");    
				var currentMediaId = "<%= BasePage.CurrentMedia.ID %>";

                if ($(this).parents(".field").length == 0)
                {
                    removeField ="<a class='remove' href='javascript:void(0)'><i class='fa fa-trash' aria-hidden='true'></i></a>"
				}				

				var shortCode = "{{Load:" + mediaId + "}.Field:" + fieldcode + "}";

				if (mediaId == currentMediaId) {
					var shortCode = "{Field:" + fieldcode + "}";
				}

                $(this).prepend("<div class='fieldControls'><a class='edit colorbox iframe' href='" + BaseUrl + "Admin/Views/PageHandlers/FieldEditor/Default.aspx?fieldId=" + fieldId + "' data-OnColorboxClose='RefreshPage()' data-width='60%' data-height='80%' title="+shortCode+"><i class='fa fa-pencil' aria-hidden='true'></i></a>"+removeField+"<div class='clear'></div></div>");
            });

            $(document).on("click", ".field .edit", function () {
                $(".field .edit").hide();
            });

            $(document).on("click", ".fieldControls .remove", function () {
                var canRemove = confirm("Are you sure you want to remove this field?");

                if (canRemove)
                {
                    $(this).closest(".field").remove();
                }
            });

            $(document).on("mouseenter", ".fieldControls a", function () {
                $(this).closest(".field").addClass("hover");
            });

            $(document).on("mouseleave", ".fieldControls a", function () {
                $(this).closest(".field").removeClass("hover");
            });            
        }

		var IsLoggedIn = false;

        $(document).ready(function () {        

            $.get("/WebServices/Site.asmx/CanAccessFrontEndEditorForMediaDetail?id=<%= BasePage.CurrentMediaDetail.ID %>", function (data) {
                if (data.IsError)
                {                                        
                    return;
                }
                else
                {
                    IsLoggedIn = true;

                    $("#LoggedInHeaderPanel").show();

                    function UpdateSliderTabIcon() {
                        if ($("#AccessCMSPermissionsPanel").is(":visible")) {
                            $("#SlideTab .fa").removeClass("fa-arrow-up");
                            $("#SlideTab .fa").addClass("fa-arrow-down");
                        }
                        else {
                            $("#SlideTab .fa").removeClass("fa-arrow-down");
                            $("#SlideTab .fa").addClass("fa-arrow-up");
                        }
                    }

					CreateFieldsEditor();

					if (typeof initVisualLayoutEditor == 'function') {
						initVisualLayoutEditor(head);
					}

                    if (GetCMSShortcutsVisibility() == "true") {
                        $("#AccessCMSPermissionsPanel").show(0, function () {
                            UpdateSliderTabIcon();
                        });
                    }
                    else if (GetCMSShortcutsVisibility() == "false") {
                        setTimeout(function () {
                            HideFieldsEditor();
                        }, 100);

                        $("#AccessCMSPermissionsPanel").hide(0, function () {
                            UpdateSliderTabIcon();
                        });
                    }

                    if (window.top != window) {
                        $("#LoggedInHeaderPanel").hide();
                        HideFieldsEditor();
                    }
                    else {
                        ShowFieldsEditor();
                    }

                    $("#SlideTab").on("click", function () {
						$("#AccessCMSPermissionsPanel").slideToggle(function () {
							if (window.location.search.toLowerCase().indexOf("visuallayouteditor=true") == -1) {
								SetCMSShortcutsVisibility($(this).is(":visible"));
							}
                        });
                    });

                    function GetCMSShortcutsVisibility() {
                        var visibility = $.cookie('CMSShortcutsVisibility');

                        if (visibility == undefined) {
                            visibility = $("#AccessCMSPermissionsPanel").is(":visible");
                            SetCMSShortcutsVisibility(visibility);
                        }

                        return visibility;
                    }

                    function SetCMSShortcutsVisibility(val) {
                        var visibility = $.cookie('CMSShortcutsVisibility', val);
                        UpdateSliderTabIcon();

                        if (val) {
                            ShowFieldsEditor();
                        }
                        else {
                            HideFieldsEditor();
                        }

                        return visibility;
                    }

                }
            });       
        });
    </script>
</asp:Panel>

<% } %>