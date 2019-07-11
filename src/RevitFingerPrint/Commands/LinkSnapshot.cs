using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace Metamorphosis
{
    [Transaction(TransactionMode.Manual), Journaling(JournalingMode.UsingCommandData)]
    public class LinkSnapshot : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            
           try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                IList<Document> inMemory = Utilities.RevitUtils.GetProjectsInMemory(commandData.Application.Application);

                IEnumerable<RevitLinkInstance> rvtLinks = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).ToElements().Cast<RevitLinkInstance>();

                IList<Document> linkDocs = rvtLinks.Select(rvt => rvt.GetLinkDocument()).ToList();


                UI.ExportSelectionForm form = new UI.ExportSelectionForm(inMemory, doc);

                if (form.ShowDialog() != DialogResult.OK) return Result.Cancelled;

                

                string filename = form.Filename;

                Document linkRvtDoc = null;

                foreach (Document linkDoc in linkDocs)
                {
                    try
                    {
                        if (linkDoc.PathName.Contains(form.SelectedDocument.PathName))
                            linkRvtDoc = linkDoc;
                    }
                    catch
                    {

                    }
                }

                SnapshotMaker maker = new SnapshotMaker(linkRvtDoc, form.Filename);
                maker.Export();

                TaskDialog td = new TaskDialog("Fingerprint");
                td.MainContent = "The snapshot file has been created.";
                td.ExpandedContent = "File: " + filename + Environment.NewLine + "Duration: " + maker.Duration.TotalMinutes.ToString("F2") + " minutes.";
                td.Show();

                GC.Collect();
                GC.WaitForPendingFinalizers();



                return Result.Succeeded;
            }            
            catch (ApplicationException aex)
            {
                MessageBox.Show(aex.Message);
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex);
                return Result.Failed;
            }
            
        }

        /// <summary>
        /// Batch version, so that others can call it.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filename"></param>
        public static void Export(Document doc, string filename)
        {
            //doc.Application.WriteJournalComment("Launching Batch Metamorphosis Snapshot...", false);
            //doc.Application.WriteJournalComment("  Filename: " + filename, false);

            SnapshotMaker maker = new SnapshotMaker(doc, filename);
            maker.Export();

            //doc.Application.WriteJournalComment("Snapshot completed. Duration:  " + maker.Duration, false);
            //doc.Application.WriteJournalComment("Garbage Collection to release db...", false);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            //doc.Application.WriteJournalComment("Garbage is collected.", false);

        }
    }
}
