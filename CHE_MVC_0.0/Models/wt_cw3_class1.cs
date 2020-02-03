using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHE_MVC_0._0.Models
{
    //[MetadataType(typeof(wt_cw3_classMetadata))]// Specifies ClassMetadata below as a the source for metadata to this class
    public partial class GUIClass
    {
        // Contains extra validation and metadata for class (wt_cw3_class)
        // Adds to the whole model
    }

    public class wt_cw3_classMetadata
    {
        [Display(Name = "Class ID")]
        [Remote("IsCidAvailable", "Class", ErrorMessage = "Class ID already used!")]
        public int cid { get; set; }

        [Required]
        [Remote("IsNameAvailable", "Class", ErrorMessage = "Name already used!")]
        public string name { get; set; }

        //[Required(ErrorMessage ="Required, Use 0 if class has no parent")]
        [Display(Name = "Parent Class ID")]
        [Remote("IsParentAvailable", "Class", ErrorMessage = "Parent Class does not exist!")]
        public int pid { get; set; }
    }
}