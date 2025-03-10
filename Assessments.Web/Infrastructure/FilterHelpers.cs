namespace Assessments.Web.Infrastructure
{
    public class FilterHelpers
    {
        public class FilterAndMetaData
        {
            /// <summary>
            /// The filters themselfes
            /// </summary>
            public FilterItem[] Filters { get; set; }

            /// <summary>
            /// Describing the filter. This will show as info text when you open a filter.
            /// </summary>
            public string FilterDescription { get; set; }

            /// <summary>
            /// A norwegian name for the button to open filters. Sub groups do not need this.
            /// </summary>
            public string FilterButtonName { get; set; }

            /// <summary>
            /// The text displayed on the button. Also known as the name of the filter.
            /// </summary>
            public string FilterButtonText { get; set; }
        }

        public class FilterItem
        {
            public string Description { get; set; }

            public string FilterHelpText { get; set; }

            public string InfoUrl { get; set; }

            public string ImageUrl { get; set; }

            public string Mapping { get; set; }

            public string Name { get; set; }

            public string NameShort { get; set; }

            public FilterAndMetaData SubGroup { get; set; }
        }
    }
}
