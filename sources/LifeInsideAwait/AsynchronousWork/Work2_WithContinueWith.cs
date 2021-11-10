﻿// C# Pills 15mg
// Copyright (C) 2021 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Threading.Tasks;
using DustInTheWind.CSharpPills.LifeInsideAwait.Utils;

namespace DustInTheWind.CSharpPills.LifeInsideAwait.AsynchronousWork
{
    internal class Work2_WithContinueWith : IWork
    {
        public virtual Task DoAsync(int stepIndex)
        {
            // before await
            ContextInfo.Display("Before await", stepIndex);

            // async call
            return Task.Delay(1000).ContinueWith(t =>
            {
                Continuation();
            });

            // after await
            void Continuation()
            {
                ContextInfo.Display("After await", stepIndex);
            }
        }
    }
}