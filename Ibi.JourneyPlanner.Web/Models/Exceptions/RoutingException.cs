// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingException.cs" company="IBI Group">
//   (c) Copyright IBI Group. Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// </copyright>
// <summary>
//   Exception classes for routing errors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ibi.JourneyPlanner.Web.Models.Exceptions
{
    using System;

    /// <summary>
    /// Exception classes for routing errors.
    /// </summary>
    public class RoutingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public RoutingException(string message)
            : base(message)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public RoutingException(string message, Exception innerException)
            : base(message, innerException)
        {            
        }
    }
}