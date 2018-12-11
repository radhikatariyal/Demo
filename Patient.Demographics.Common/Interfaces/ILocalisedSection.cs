using System;
using System.Collections.Generic;

namespace Patient.Demographics.Common.Interfaces
{
    public interface ILocalisedSection
    {
        int SectionId { get; set; }

        string Culture { get; set; }

        string Content { get; set; }
    }
}