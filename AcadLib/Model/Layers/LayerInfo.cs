﻿using AcadLib.Colors;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using JetBrains.Annotations;
using NetLib;
using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace AcadLib.Layers
{
    [PublicAPI]
    [Serializable]
    [Equals(DoNotAddEqualityOperators = true)]
    public class LayerInfo
    {
        private Color color;
        private string colorStr;
        private LineWeight? lineWeight;

        [XmlIgnore]
        [JsonIgnore]
        [IgnoreDuringEquals]
        public Color Color
        {
            get => color;
            set {
                color = value;
                colorStr = color.AcadColorToString2();
            }
        }
        /// <summary>
        /// Только для Serializable
        /// </summary>
        [PublicAPI]
        public string ColorStr
        {
            get => colorStr;
            set {
                colorStr = value;
                color = colorStr.AcadColorFromString2();
            }
        }
        
        public bool IsFrozen { get; set; }
        
        public bool IsLocked { get; set; }
        
        public bool IsOff { get; set; }
        
        public bool IsPlotable { get; set; } = true;

        [XmlIgnore]
        [JsonIgnore]
        [IgnoreDuringEquals]
        public ObjectId LayerId { get; set; }
        
        public string LineType { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [IgnoreDuringEquals]
        public ObjectId LinetypeObjectId { get; set; }
        
        public LineWeight LineWeight
        {
            get => lineWeight ?? LineWeight.ByLayer;
            set => lineWeight = value;
        }
        
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;

        public LayerInfo()
        {
        }

        public LayerInfo(string name)
        {
            Name = name;
            Color = Color.FromColorIndex(ColorMethod.ByAci, 7);
            LineWeight = LineWeight.ByLineWeightDefault;
        }

        public LayerInfo(ObjectId idLayer)
        {
#pragma warning disable 618
            using (var layer = (LayerTableRecord)idLayer.Open(OpenMode.ForRead))
#pragma warning restore 618
            {
                Name = layer.Name;
                Color = layer.Color;
#pragma warning disable 618
                using (var lt = (LinetypeTableRecord)layer.LinetypeObjectId.Open(OpenMode.ForRead))
#pragma warning restore 618
                {
                    LineType = lt.Name;
                }
                LineWeight = layer.LineWeight;
                IsPlotable = layer.IsPlottable;
                Description = layer.Description;
            }
        }

        /// <summary>
        /// Установка свойст LayerInfo к слою LayerTableRecord
        /// </summary>
        /// <param name="lay"></param>
        /// <param name="db">Чертеж</param>
        public void SetProp([NotNull] LayerTableRecord lay, Database db)
        {
            if (!Name.IsNullOrEmpty()) lay.Name = Name;
            if (Color != null) lay.Color = Color;
            lay.IsFrozen = IsFrozen;
            lay.IsLocked = IsLocked;
            lay.IsOff = IsOff;
            lay.IsPlottable = IsPlotable;
            lay.Description = Description;
            if (lineWeight.HasValue) lay.LineWeight = LineWeight;
            if (!LinetypeObjectId.IsNull) lay.LinetypeObjectId = LinetypeObjectId;
            else if (!string.IsNullOrEmpty(LineType)) lay.LinetypeObjectId = db.GetLineTypeIdByName(LineType);
        }
    }
}