//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PESUpdateData
{
    using System;
    using System.Collections.Generic;
    
    public partial class DataForEvaluations
    {
        public int Id { get; set; }
        public decimal Expect { get; set; }
        public decimal Result { get; set; }
        public Nullable<decimal> OldResult { get; set; }
        public string AuditComment { get; set; }
        public int Approve { get; set; }
        public string CommentApprove { get; set; }
        public int PointOfEvaluationId { get; set; }
        public int OfficeId { get; set; }
        public string UpdateUserId { get; set; }
        public int Month { get; set; }
        public string AttachFile { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual Offices Offices { get; set; }
        public virtual PointOfEvaluations PointOfEvaluations { get; set; }
    }
}
