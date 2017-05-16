using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public static class Pathfinder
    {
        class State :IComparable
        {
            public Field field;
            public float priority;
            public List<int> solution;
            public State(Field f, List<int> s, Field d)
            {
                field = f;
                solution = s;
                solution.Add(field.Index);
                priority = getF(d);
            }

            public void addStep(int x)
            {
                solution.Add(x);
            }

            private float getF(Field destination)
            {
                return Heuristic(this.field.Position, destination.Position) + (solution.Count * 7.2f);
            }

            private float Heuristic(Vector3 curr, Vector3 destiny)
            {
                Vector3 dif = destiny - curr;
                return abs(destiny.X) + abs(destiny.Y);
            }

            private float abs(float x)
            {
                if (x < 0) return -x;
                else return x;
            }

            public int CompareTo(object x)
            {
                if (x == null) return 0;
                State s = x as State;
                return s.priority.CompareTo(this.priority);
            }
        }

        public static List<int> FindPath(Field start, Field destination)
        {
            Logger.Log.Debug("Pathfinding...");
            List<int> path = new List<int>();
            //int[] order = new int[] { 2, 3, 0, 1, 4, 5 };
            List<State> frontier = new List<State>();
            HashSet<Field> visited = new HashSet<Field>();
            frontier.Add(new State(start, path, destination));

            State current = null;
            return path;
            while (frontier.Count > 0)
            {
                current = frontier.Last();
                frontier.Remove(frontier.Last());
                //frontier.RemoveAt(0);
                visited.Add(current.field);
                if (current.field.Index == destination.Index)
                {
                    return current.solution;
                }
                else
                {
                    List<Field> near = current.field.GetNearestFields();
                    for (int i = 0; i < 6; i++)
                    {
                        if (near.ElementAt(i) != null && destination.CanPlaceUnit())
                        {
                            State s = new State(near.ElementAt(i), current.solution, destination);
                            frontier.Add(s);
                            frontier.Sort();
                        }
                    }
                }
            }
            return path;
        }
    }
}
