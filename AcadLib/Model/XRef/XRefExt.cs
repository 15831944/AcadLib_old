﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace AcadLib.XRef
{
    public static class XRefExt
    {
        /// <summary>
        /// Attaches the specified Xref to the current space in the current drawing.
        /// </summary>
        /// <param name="path">Path to the drawing file to attach as an Xref.</param>
        /// <param name="pos">Position of Xref in WCS coordinates.</param>
        /// <param name="name">Optional name for the Xref.</param>
        /// <param name="idBtrXref">Блок внешней ссылки - BlockTableRecord</param>
        /// <returns>Whether the attach operation succeeded.</returns>
        public static bool XrefAttachAndInsert (this Database db, string path, Point3d pos, 
            out ObjectId idBtrXref, out ObjectId idBlRefXref, string name = null)
        {
            idBtrXref = ObjectId.Null;
            idBlRefXref = ObjectId.Null;
            var ret = false;
            if (!File.Exists(path))
                return ret;
            if (String.IsNullOrEmpty(name))
                name = Path.GetFileNameWithoutExtension(path);

            using (var t = db.TransactionManager.StartOpenCloseTransaction())
            {
                idBtrXref = db.AttachXref(path, name);
                if (idBtrXref.IsValid)
                {
                    var ms = (BlockTableRecord)t.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite);
                    var br = new BlockReference(pos, idBtrXref);
                    idBlRefXref = ms.AppendEntity(br);
                    t.AddNewlyCreatedDBObject(br, true);
                    ret = true;
                }
                t.Commit();
            }
            return ret;
        }
    }
}
