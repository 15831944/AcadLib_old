﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Autodesk.AutoCAD.DatabaseServices
{
   public static class ExtentsExtension
   {
      /// <summary>
      /// Определение точки центра границы Extents3d
      /// </summary>
      /// <param name="ext"></param>
      /// <returns></returns>
      public static Point3d Center (this Extents3d ext)
      {
         return new  Point3d((ext.MaxPoint.X + ext.MinPoint.X) * 0.5,
                             (ext.MaxPoint.Y + ext.MinPoint.Y) * 0.5, 0);
      }

      /// <summary>
      /// Длина диагонали границ (расстояние между точками MaxPoint и MinPoint)
      /// </summary>
      /// <param name="ext"></param>
      /// <returns></returns>
      public static double Diagonal (this Extents3d ext)
      {
         return (ext.MaxPoint - ext.MinPoint).Length;         
      }

      /// <summary>
      /// Попадает ли точка внутрь границы
      /// </summary>      
      /// <returns></returns>
      public static bool IsPointInBounds(this Extents3d ext, Point3d pt)
      {
         bool res = false;
         if (pt.X > ext.MinPoint.X && pt.Y > ext.MinPoint.Y &&
            pt.X < ext.MaxPoint.X && pt.Y < ext.MaxPoint.Y)
         {
            res = true;
         }
         return res;
      }

      /// <summary>
      /// Попадает ли точка внутрь границы
      /// </summary>      
      /// <returns></returns>
      public static bool IsPointInBounds(this Extents3d ext, Point3d pt, double tolerance)
      {
         bool res = false;
         if ((pt.X-tolerance) > ext.MinPoint.X && (pt.Y-tolerance) > ext.MinPoint.Y &&
            (pt.X+tolerance) < ext.MaxPoint.X && (pt.Y+tolerance) < ext.MaxPoint.Y)
         {
            res = true;
         }
         return res;
      }
   }
}