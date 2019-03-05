﻿/*
 * This file is part of the CatLib package.
 *
 * (c) CatLib <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://catlib.io/
 */

using System;

namespace CatLib
{
    /// <summary>
    /// Marked classes, functions, attributes no longer perform coverage analysis.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor, Inherited = false)]
    public class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }
}
