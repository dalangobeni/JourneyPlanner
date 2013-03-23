﻿// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

namespace Ibi.JourneyPlanner.Web.Code.Language
{
    using System.Collections.Generic;

    using OsmSharp.Routing.Core.ArcAggregation.Output;
    using OsmSharp.Routing.Instructions;
    using OsmSharp.Routing.Instructions.LanguageGeneration;
    using OsmSharp.Tools.Math.Geo.Meta;

    /// <summary>
    /// A simple instruction generator, translating instructions into the english language.
    /// </summary>
    public class SimpleEnglishLanguageGenerator : ILanguageGenerator
    {
        private string TurnDirection(RelativeDirectionEnum direction)
        {
            switch (direction)
            {
                case RelativeDirectionEnum.Right:
                case RelativeDirectionEnum.SharpRight:
                case RelativeDirectionEnum.SlightlyRight:
                    return "right";
                case RelativeDirectionEnum.Left:
                case RelativeDirectionEnum.SharpLeft:
                case RelativeDirectionEnum.SlightlyLeft:
                    return "left";
                case RelativeDirectionEnum.TurnBack:
                    return "back";
            }
            return string.Empty;
        }

        #region ILanguageGenerator Members

        public string AddOrdinal(int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num.ToString() + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num.ToString() + "st";
                case 2:
                    return num.ToString() + "nd";
                case 3:
                    return num.ToString() + "rd";
                default:
                    return num.ToString() + "th";
            }

        }

        /// <summary>
        /// Generates an instruction for a direct turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="street_count_before_turn"></param>
        /// <param name="street_to"></param>
        /// <param name="direction"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateDirectTurn(Instruction instruction, int street_count_before_turn,
            List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<PointPoi> list)
        {
            if (street_count_before_turn == 1)
            {
                instruction.Text = string.Format("Take the first turn {0}, on {1}.",
                    this.TurnDirection(direction),
                    this.GetName("en", street_to));
            }
            else
            {
                instruction.Text = string.Format("Take the {0} turn {1}, on {2}.",
                    AddOrdinal(street_count_before_turn),
                    this.TurnDirection(direction),
                    this.GetName("en", street_to));
            }

            // returns the instruction with text.
            return instruction;
        }

        /// <summary>
        /// Generates an instruction for an indirect turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="street_count_turn"></param>
        /// <param name="street_count_before_turn"></param>
        /// <param name="street_to"></param>
        /// <param name="direction"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateIndirectTurn(Instruction instruction, int street_count_turn, int street_count_before_turn,
            List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<PointPoi> list)
        {
            instruction.Text = string.Format("Take the {0} turn {1}, on {2}.",
                AddOrdinal(street_count_before_turn),
                this.TurnDirection(direction),
                this.GetName("en", street_to));

            // returns the instruction with text.
            return instruction;
        }

        /// <summary>
        /// Generates an instruction for a POI.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="list"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Instruction GeneratePoi(Instruction instruction, List<PointPoi> list, RelativeDirectionEnum? direction)
        {
            if (direction == null)
            {
                instruction.Text = string.Format("Poi");
            }
            else
            {
                instruction.Text = string.Format("Poi: {0}", direction);
            }

            // returns the instruction with text.
            return instruction;
        }

        /// <summary>
        /// Generates an instruction for a turn followed by another turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="street_count_before_turn"></param>
        /// <param name="street_to"></param>
        /// <param name="direction"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateDirectFollowTurn(Instruction instruction, int street_count_before_turn, List<KeyValuePair<string, string>> street_to,
            RelativeDirectionEnum direction, List<PointPoi> list)
        {
            if (street_count_before_turn == 1)
            {
                instruction.Text = string.Format("Turn {1} to stay on {0}.",
                    this.GetName("en", street_to),
                    this.TurnDirection(direction));
            }
            else
            {
                instruction.Text = string.Format("Turn {1} street {2} to stay on {0}.",
                    this.GetName("en", street_to),
                    AddOrdinal(street_count_before_turn),
                    this.TurnDirection(direction));
            }

            // returns the instruction with text.
            return instruction;
        }

        /// <summary>
        /// Generates an instruction for an indirect turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="street_count_turn"></param>
        /// <param name="street_count_before_turn"></param>
        /// <param name="street_to"></param>
        /// <param name="direction"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateIndirectFollowTurn(Instruction instruction, int street_count_turn, int street_count_before_turn, List<KeyValuePair<string, string>> street_to,
            RelativeDirectionEnum direction, List<PointPoi> list)
        {
            if (street_count_before_turn == 1)
            {
                instruction.Text = string.Format("Turn {1} to stay on {0}.",
                    this.GetName("en", street_to),
                    this.TurnDirection(direction));
            }
            else
            {
                instruction.Text = string.Format("Take the {1} street {2} to stay on {0}.",
                    this.GetName("en", street_to),
                    AddOrdinal(street_count_before_turn),
                    this.TurnDirection(direction));
            }

            // returns the instruction with text.
            return instruction;
        }

        /// <summary>
        /// Generates an instruction for an immidiate turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="first_street_count_to"></param>
        /// <param name="first_street_to"></param>
        /// <param name="first_direction"></param>
        /// <param name="second_street_to"></param>
        /// <param name="second_direction"></param>
        /// <returns></returns>
        public Instruction GenerateImmidiateTurn(Instruction instruction, int first_street_count_to, List<KeyValuePair<string, string>> first_street_to,
            RelativeDirection first_direction, List<KeyValuePair<string, string>> second_street_to, RelativeDirection second_direction)
        {
            if (first_street_count_to == 1)
            {
                instruction.Text = string.Format("Take the first turn {0}, on the {1}, and turn immidiately {2} on the {3}.",
                    this.TurnDirection(first_direction.Direction),
                    this.GetName("en", first_street_to),
                    this.TurnDirection(second_direction.Direction),
                    this.GetName("en", second_street_to));
            }
            else
            {
                instruction.Text = string.Format("Take the {4} turn {0}, on the {1}, and turn immidiately {2} on the {3}.",
                    this.TurnDirection(first_direction.Direction),
                    this.GetName("en", first_street_to),
                    this.TurnDirection(second_direction.Direction),
                    this.GetName("en", second_street_to),
                    AddOrdinal(first_street_count_to));
            }

            // returns the instruction with text.
            return instruction;
        }

        /// <summary>
        /// Generates an instruction for a roundabout.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="count"></param>
        /// <param name="next_street"></param>
        /// <returns></returns>
        public Instruction GenerateRoundabout(Instruction instruction, int count, List<KeyValuePair<string, string>> next_street)
        {
            instruction.Text = string.Format("Take the {0} at the next roundabout on the {1}.",
                AddOrdinal(count),
                this.GetName("en", next_street));

            // returns the instruction with text.
            return instruction;
        }

        /// <summary>
        /// Generates an instruction for a simple turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Instruction GenerateSimpleTurn(Instruction instruction, RelativeDirectionEnum direction)
        {
            instruction.Text = string.Format("Turn {0}", this.TurnDirection(direction));

            return instruction;
        }

        #endregion

        private string GetName(string language_key, List<KeyValuePair<string, string>> tags)
        {
            language_key = language_key.ToLower();

            string name = string.Empty;
            foreach (KeyValuePair<string, string> tag in tags)
            {
                if (tag.Key != null && tag.Key.ToLower() == string.Format("name:{0}", language_key))
                {
                    return tag.Value;
                }

                if (tag.Key != null && tag.Key.ToLower() == "name")
                {
                    name = tag.Value;
                }
            }

            return name;
        }
    }
}