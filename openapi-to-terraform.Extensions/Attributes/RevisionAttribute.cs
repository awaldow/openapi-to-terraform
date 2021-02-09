using System;

namespace openapi_to_terraform.Extensions.Attributes
{
    /// <summary>
    /// Use RevisionAttribute either on class inheriting ControllerBase or Method on ControllerBase
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class)]
    public class RevisionsAttribute : Attribute
    {
        public int[] Revisions
        {
            get
            {
                return _revisions;
            }
        }

        private int[] _revisions { get; set; }

        public RevisionsAttribute(params int[] revisions)
        {
            _revisions = revisions;
        }
    }
}