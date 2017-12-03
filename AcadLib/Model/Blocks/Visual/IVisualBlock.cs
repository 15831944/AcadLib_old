﻿using MicroMvvm;
using System.Windows.Media;

namespace AcadLib.Blocks.Visual
{
    public interface IVisualBlock
    {
        string Group { get; set; }
        string Name { get; set; }
        ImageSource Image { get; set; }
        string File { get; set; }
        RelayCommand Redefine { get; set; }        
    }
}
