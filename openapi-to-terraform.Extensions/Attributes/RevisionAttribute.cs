using System;

namespace openapi_to_terraform.Extensions.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class RevisionAttribute : Attribute
    {
        public int[] Revisions
        {
            get
            {
                return _revisions;
            }
        }

        private int[] _revisions { get; set; }
        
        public RevisionAttribute(int[] revisions)
        {
            _revisions = revisions;
        }
    }
}