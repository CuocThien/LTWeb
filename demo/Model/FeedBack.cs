//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace demo.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class FeedBack
    {
        public string ID_Order { get; set; }
        public string FeedBack1 { get; set; }
    
        public virtual Order Order { get; set; }
    }
}
