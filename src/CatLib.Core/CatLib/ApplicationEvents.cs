﻿/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

namespace CatLib
{
    /// <summary>
    /// 应用程序事件
    /// </summary>
    public sealed class ApplicationEvents
    {
        /// <summary>
        /// 当引导程序开始之前
        /// </summary>
        public static readonly string OnBootstrap = "CatLib.ApplicationEvents.OnBootstrap";

        /// <summary>
        /// 当引导程序进行中
        /// </summary>
        public static readonly string Bootstrapping = "CatLib.ApplicationEvents.Bootstrapping";

        /// <summary>
        /// 当引导完成时
        /// </summary>
        public static readonly string OnBootstraped = "CatLib.ApplicationEvents.OnBootstraped";

        /// <summary>
        /// 当注册服务提供者
        /// </summary>
        public static readonly string OnRegisterProvider = "CatLib.ApplicationEvents.OnRegisterProvider";

        /// <summary>
        /// 当初始化开始之前
        /// </summary>
        public static readonly string OnInit = "CatLib.ApplicationEvents.OnInit";

        /// <summary>
        /// 当初始化进行时
        /// </summary>
        [System.Obsolete("Please use OnProviderInit")]
        public static readonly string OnIniting = "CatLib.ApplicationEvents.OnProviderInit";

        /// <summary>
        /// 当服务提供者初始化进行前
        /// </summary>
        public static readonly string OnProviderInit = "CatLib.ApplicationEvents.OnProviderInit";

        /// <summary>
        /// 当服务提供者初始化结束后
        /// </summary>
        public static readonly string OnProviderInited = "CatLib.ApplicationEvents.OnProviderInited";

        /// <summary>
        /// 当初始化完成之后
        /// </summary>
        public static readonly string OnInited = "CatLib.ApplicationEvents.OnInited";

        /// <summary>
        /// 当程序启动完成
        /// </summary>
        public static readonly string OnStartCompleted = "CatLib.ApplicationEvents.OnStartCompleted";

        /// <summary>
        /// 当程序终止之前
        /// </summary>
        public static readonly string OnTerminate = "CatLib.ApplicationEvents.OnTerminate";

        /// <summary>
        /// 当程序终止之后
        /// </summary>
        public static readonly string OnTerminated = "CatLib.ApplicationEvents.OnTerminated";
    }
}