using DotSpatial.Controls;
using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TASK_4_FIXXX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Clear the existing layers from the map control
            if ((map1.Layers.Count > 0))
            {
                map1.Layers.Clear();
            }
            //Clear the exisiting items from the combobox
            cmbFiledName.Items.Clear();
            //add the layers
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Shapefiles|*.shp";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //add layer to first map
                FeatureSet featureSet1 = (FeatureSet)
                FeatureSet.Open(fileDialog.FileName);
                //FillColumnNames method is used to get all the attribute column names 
                //Based on the names combobox will be populated
                FillColumnNames(featureSet1);
                featureSet1.Reproject(map1.Projection);
                map1.Layers.Add(featureSet1);
                map1.ZoomToMaxExtent();
            }
        }

        private void FillColumnNames(IFeatureSet featureSet)
        {
            foreach (DataColumn column in featureSet.DataTable.Columns)
            {
                cmbFiledName.Items.Add(column.ColumnName);
            }
        }

        string fname = "Tahoma";
        double fsize = 8.0;
        Color fcolor = Color.Black;

        private void btnDisplayLabel_Click(object sender, EventArgs e)
        {
            if ((cmbFiledName.Text == string.Empty))
            {
                MessageBox.Show("Please select an attribute from the textbox");
            }
            else
            {
                DisplayLabels(cmbFiledName.Text.ToString());
            }
        }
        private void DisplayLabels(string attributename)
        {
            //Check the number of layers from MapControl
            if (map1.Layers.Count > 0)
            {
                map1.AddLabels((DotSpatial.Symbology.FeatureLayer)map1.Layers[0], "[" +
               attributename + "]", new Font("" + fname + "", (float)fsize), fcolor);
            }
            else
            {
                MessageBox.Show("Please add a layer to the map.");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DisplayCustomLabel(txtCustomAttribute.Text);
            txtCustomAttribute.Text = "";
        }
        private void DisplayCustomLabel(string attributeValue)
        {
            if (string.IsNullOrEmpty(txtCustomAttribute.Text))
            {
                MessageBox.Show("Please enter the label text");
                return;
            }
            IMapFeatureLayer selectedLayer = (IMapFeatureLayer)map1.Layers[0];
            if (selectedLayer == null)
            {
                MessageBox.Show("Please add a layer to the map");
                return;
            }
            int numSelectedFeatures = selectedLayer.Selection.Count;
            if (numSelectedFeatures == 0)
            {
                MessageBox.Show("Please select a shape in the map");
                return;
            }
            //create new column in attribute table
            DataTable table = selectedLayer.DataSet.DataTable;
            if (!(table.Columns.Contains("new_label")))
            {
                table.Columns.Add(new DataColumn("new_label"));
            }
            List<IFeature> selectedFeatureList = selectedLayer.Selection.ToFeatureList();
            IFeature selectedFeature = selectedFeatureList[0];
            selectedFeature.DataRow["new_label"] = txtCustomAttribute.Text;
            //display labels in the map
            map1.AddLabels(selectedLayer, "[new_label]", new Font("" + fname + "", (float)fsize),
           fcolor);
            //reset map selection mode
            map1.FunctionMode = FunctionMode.None;
        }

        private void txtCustomAttribute_TextChanged(object sender, EventArgs e)
        {
            map1.FunctionMode = FunctionMode.Select;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (map1.Layers.Count > 0)
            {
                IMapFeatureLayer stateLayer = default(IMapFeatureLayer);
                stateLayer = (IMapFeatureLayer)map1.Layers[0];
                stateLayer.DataSet.Save();
                MessageBox.Show("Attribute has been saved in the shapefile.");
            }
            else
            {
                MessageBox.Show("Please add a layer to the map.");
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataTable dt = null;
            if (map1.Layers.Count > 0)
            {
                IMapFeatureLayer stateLayer = default(IMapFeatureLayer);
                stateLayer = (IMapFeatureLayer)map1.Layers[0];
                //Get the shapefile's attribute table to our datatable dt
                dt = stateLayer.DataSet.DataTable;
                //Remove a column
                dt.Columns.Remove("new_label");
                stateLayer.DataSet.Save();
                MessageBox.Show("Attribute has been removed in the shapefile.");
            }
            else
            {
                MessageBox.Show("Please add a layer to the map.");
            }
        }

        private void btnsetFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fname = fontDialog1.Font.Name;
                fsize = fontDialog1.Font.Size;
            }
        }

        private void btnsetColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                fcolor = colorDialog1.Color;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map1.Layers.Clear();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Confirm with users that they are ready to close application or not with the help of message box.
            if (MessageBox.Show("Do you want to close this application?", "Admin", MessageBoxButtons.OKCancel) ==
           DialogResult.OK)
            {
                //Close() method is used to close the application.
                this.Close();
            }
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map1.ZoomIn();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map1.ZoomOut();
        }

        private void zoomToExtendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map1.ZoomToMaxExtent();
        }

        private void panToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map1.FunctionMode = FunctionMode.Pan;
        }

        private void measureToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // map1.FunctionMode = FunctionMode.Measure;
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map1.FunctionMode = FunctionMode.None;
        }
    }
}
