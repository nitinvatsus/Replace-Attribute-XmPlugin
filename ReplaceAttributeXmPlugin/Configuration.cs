using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace ReplaceAttributeXmPlugin
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Replace/Delete Attribute from View and Form "),
        ExportMetadata("Description", "This plugin delete or replace source attribute from destination attribute in forms and View"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAG8SURBVFhHY/wPBAwDCJigNMmAUXIClEUZINsB1AJD0wGS+rOhLMoBWQ548eorlEU5oGkUEJNQSXYAsal/+fqbUBZ+QLMQiMraDmXhByQ5YMvu+1AWflDZdhTKIgxIcoBv3EYoCz/omHwayiIMiHaAsOYMKAs/ICaNIKsh2gHvPvyAsnADQpa/ff8DQw1RDmCVnQRlUQZEtDBDkaADzl95xfDnzz8oDzfA53uQHC55gg4wcl0GZeEGhCzHB4hOA7gAIQsIAYodQCnA6wBKfUcMwOkAbqUpUBZtAVYHfPz0k+Hb9z9QHm0BVgcIqE+HsmgPMBxAj3hHBigOIGR5Wowuw//nBWD89noGQ1zuTqgM+QDeL8BlubgoF8PL19+gPFTAzsYMpn/++gumSQEgT4AAOATQLfdzV2IQ5GcHs19cSmNor7IGs9EByGJcloP0wSzBBxgZJPrhPaNbRxMY1KwXMGxd4s/gHYNa98eFajLUFVkwKCvwQ0VQwYPHnxjyag4wbN51DyqCH8AcB3aAnDQvw/ROJ7ClakqCDDePxoMlqQWwRS88dDjkJ4OSAV0ByNMwMHQ7p9QCA+wABgYARE301l0ehhoAAAAASUVORK5CYII="),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class Configuration : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new ReplaceAttributeControl();
        }
    }
}