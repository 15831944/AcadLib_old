﻿using System.Collections.Generic;
using AcadLib;
using AcadLib.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace TestAcadlib.Geometry.Polylines
{
    public class TestMergePolyline
    {
        [CommandMethod(nameof(TestTestMergePolyline))]
        public void TestTestMergePolyline()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var selRes = ed.Select("Выбери полилинии для объединения");
            if (selRes.Count == 0)
                return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var pls = new List<Polyline>();
                foreach (var item in selRes)
                {
                    if (!item.IsValidEx())
                        continue;
                    var pl = item.GetObject(OpenMode.ForRead) as Polyline;
                    if (pl != null)
                    {
                        pls.Add(pl);
                    }
                }

                try
                {
                    var plMerged = pls.Merge();

                    var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                    cs.AppendEntity(plMerged);
                    t.AddNewlyCreatedDBObject(plMerged, true);
                    plMerged.ColorIndex = 5;
                }
                catch(System.Exception ex)
                {
                    Application.ShowAlertDialog(ex.ToString());
                }

                t.Commit();
            }
        }
    }
}
