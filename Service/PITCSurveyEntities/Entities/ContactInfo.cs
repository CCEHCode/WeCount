using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PITCSurveyEntities.Entities
{
    [Table("ContactInfos")]
    public class ContactInfo
    {
        [Key]
        public int ID { get; set; }

        public string Notes { get; set; }

        public virtual IList<Contact> Contacts {get; set; }
    }
}
