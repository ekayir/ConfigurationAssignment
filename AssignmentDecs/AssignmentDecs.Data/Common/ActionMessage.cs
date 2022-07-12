using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentDecs.Data
{
    public static class ActionMessage
    {
        //[note that it cannot be defined enum whose value is string so i defined static class]
        public static readonly string NotNullMessage = "Parameter cannot be null.";
        public static readonly string AddingSuccessfullMessage = "Adding new record is successful.";
        public static readonly string UpdatingSuccessfullMessage = "Updating the record is successful.";
        public static readonly string IncompatibleTypeAndValueMessage = "Type and Value are incompatible with each other.";
        public static readonly string NoConfiguratonRelatedNameMessage = "No configuration related to {0}";
        public static readonly string ExistAnotherActiveRecordMessage = "There is another active record.";
    }
}
