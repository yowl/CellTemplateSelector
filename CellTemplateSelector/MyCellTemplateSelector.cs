using System.Windows;
using Telerik.Windows.Controls;

namespace CellTemplateSelector
{
    public class MyCellTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var club = item as DurationEntity;

            if (club != null && club.Overlapped)
            {
                return overlappedStyle;
            }
            return normalStyle;
        }

        public DataTemplate overlappedStyle { get; set; }
        public DataTemplate normalStyle { get; set; }
    }
}
