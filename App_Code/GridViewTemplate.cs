using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for GridViewTemplate
/// </summary>
/// 
namespace Delve
{
    public delegate void TextChangedMethod(object sender, EventArgs e);
    public class GridViewTemplate : ITemplate
    {
        private DataControlRowType templateType;
        private string columnNameFriendly;
        private string columnNameData;
        private Control control;
        private string Size;

        public GridViewTemplate(DataControlRowType type, string colNameDt, Control con,string size)
        {
            templateType = type;
            columnNameData = colNameDt;
            control = con;
            Size = size;
        }
        public void InstantiateIn(System.Web.UI.Control container)
        {
            switch (templateType)
            {
                case DataControlRowType.Header:
                    {
                        Literal lc = new Literal();
                        lc.Text = columnNameData;
                        container.Controls.Add(lc);
                        break;
                    } 
                case DataControlRowType.DataRow:
                    {
                        Control field = control;
                        if (field.GetType() == typeof(Label))
                        {
                            Label lbl = new Label();
                            lbl.ID = "lbl" + columnNameData;
                            if (Size != "")
                            {
                                lbl.Width = Unit.Pixel(int.Parse(Size));
                            }
                            lbl.DataBinding += new EventHandler(this.lbl_DataBind);
                            container.Controls.Add(lbl);
                        }
                        else if (field.GetType() == typeof(TextBox))
                        {
                            TextBox txt = new TextBox();
                            txt.ID = "txt" + columnNameData;
                            if (Size != "")
                            {
                                txt.Width = Unit.Pixel(int.Parse(Size));
                            }
                            txt.DataBinding += new EventHandler(this.txt_DataBind);
                            container.Controls.Add(txt);
                        }
                        else if (field.GetType() == typeof(DropDownList))
                        {
                            DropDownList ddl = (DropDownList)field;
                            ddl.ID = "ddl" + columnNameData;
                            if (Size != "")
                            {
                                ddl.Width = Unit.Pixel(int.Parse(Size));
                            }
                            ddl.DataBinding += new EventHandler(this.ddl_DataBind);
                            container.Controls.Add(ddl);
                        }
                        else if (field.GetType() == typeof(CheckBox))
                        {
                            CheckBox cbx = new CheckBox();
                            cbx.ID = "cbx" + columnNameData;
                            cbx.DataBinding += new EventHandler(this.cbx_DataBind);
                            container.Controls.Add(cbx);
                        }
                        break;
                    }
                case DataControlRowType.Footer:
                    {
                        Control field = control;
                        if (field.GetType() == typeof(TextBox))
                        {
                            TextBox txt = new TextBox();
                            txt.ID = "txt" + columnNameData;
                            if (Size != "")
                            {
                                txt.Width = Unit.Pixel(int.Parse(Size));
                            }
                            
                            txt.DataBinding += new EventHandler(this.txt_DataBind);
                            container.Controls.Add(txt);
                        }
                        else if (field.GetType() == typeof(DropDownList))
                        {
                            DropDownList ddl = (DropDownList)field;
                            ddl.ID = "ddl" + columnNameData;
                            if (Size != "")
                            {
                                ddl.Width = Unit.Pixel(int.Parse(Size));
                            }
                            ddl.DataBinding += new EventHandler(this.ddl_DataBind);
                            container.Controls.Add(ddl);
                        }
                        else if (field.GetType() == typeof(CheckBox))
                        {
                            CheckBox cbx = new CheckBox();
                            cbx.ID = "cbx" + columnNameData;
                            cbx.DataBinding += new EventHandler(this.cbx_DataBind);
                            container.Controls.Add(cbx);
                        }
                        break;
                    }
            } 
        }
        
        private void txt_DataBind(Object sender, EventArgs e)
        {
            //object bound_value_obj = null;
            //Control ctrl = (Control)sender;
            //IDataItemContainer data_item_container = (IDataItemContainer)ctrl.NamingContainer;
            //bound_value_obj = DataBinder.Eval(data_item_container.DataItem, columnNameData);
            //TextBox field_txtbox = (TextBox)sender;
            //field_txtbox.Text = bound_value_obj.ToString();

            TextBox txt = (TextBox)sender;
            GridViewRow row = (GridViewRow)txt.NamingContainer;
            if (row.RowType == DataControlRowType.Footer)
            {
                txt.Text = "";
            }
            else
            {
                txt.Text = DataBinder.Eval(row.DataItem, columnNameData).ToString();
            }
        }

        private void lbl_DataBind(Object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            GridViewRow row = (GridViewRow)lbl.NamingContainer;
            lbl.Text = DataBinder.Eval(row.DataItem, columnNameData).ToString();
            //object bound_value_obj = null;
            //Control ctrl = (Control)sender;
            //IDataItemContainer data_item_container = (IDataItemContainer)ctrl.NamingContainer;
            //bound_value_obj = DataBinder.Eval(data_item_container.DataItem, columnNameData);
            //Label field_txtbox = (Label)sender;
            //field_txtbox.Text = bound_value_obj.ToString();
        }
        private void ddl_DataBind(Object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;
            if (row.RowType == DataControlRowType.Footer)
            {
                ddl.SelectedIndex = -1;
            }
            else
            {
                ddl.SelectedValue = DataBinder.Eval(row.DataItem, columnNameData).ToString();
            }
        }

        private void cbx_DataBind(Object sender, EventArgs e)
        {
            CheckBox cbx = (CheckBox)sender;
            GridViewRow row = (GridViewRow)cbx.NamingContainer;
            cbx.Checked = (bool)DataBinder.Eval(row.DataItem, columnNameData);
        }


      
    }
}