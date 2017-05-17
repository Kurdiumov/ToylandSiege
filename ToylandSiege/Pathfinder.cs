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
                solution = new List<int>(s);
                solution.Add(field.Index);
                priority = getF(d);
                //Logger.Log.Debug("F wynosi:" + priority);
            }

            public void addStep(int x)
            {
                solution.Add(x);
            }

            private float getF(Field destination)
            {
                //float x = solution.Count * 13.198f;
                //Logger.Log.Debug("Droga :" + x);

                // Odleglosc od srodkow fieldow: 13.198
                return (Heuristic(this.field.Position, destination.Position) + solution.Count);
            }

            private float Heuristic(Vector3 curr, Vector3 destiny)
            {
                float x = (float)Math.Sqrt( ((destiny.X - curr.X) * (destiny.X - curr.X)) + ((destiny.Y - curr.Y) * (destiny.Y - curr.Y)) );
                //Logger.Log.Debug("Heurystyka: "+x);
                return x;
            }
            


            private float abs(float x)
            {
                if (x < 0) return -x;
                else return x;
            }

            public int CompareTo(object x)
            {
                if (x == null) return 0;
                State s = (State)x;
                return this.priority.CompareTo(s.priority);
            }
        }

        public static List<int> FindPath(Field start, Field destination)
        {
            DateTime begin = DateTime.Now;
            Logger.Log.Debug("Pathfinding...");
            List<int> path = new List<int>();
            //int[] order = new int[] { 2, 3, 0, 1, 4, 5 };
            List<State> frontier = new List<State>();
            HashSet<Field> visited = new HashSet<Field>();
            frontier.Add(new State(start, path, destination));
            int a = 0;
            int wygenerowane = 1;
            int przetworzone = 0;
            State current = null;
            while (frontier.Count > 0)
            {
                a++;
                int ind = 0;
                float minPrio = frontier.ElementAt(ind).priority;
                for(int j = 0; j < frontier.Count; j++)
                {
                    if(frontier.ElementAt(j).priority < minPrio)
                    {
                        minPrio = frontier.ElementAt(j).priority;
                        ind = j;
                    }

                }
                current = frontier.ElementAt(ind);
                frontier.RemoveAt(ind);

                //Logger.Log.Debug("powtorzenie " + a + " dlugosc: " + current.solution.Count);
                //current = frontier.Last();
                //frontier.Remove(frontier.Last());

                
                if(!visited.Contains(current.field))
                {
                    przetworzone++;
                    visited.Add(current.field);
                    if (current.field.Index == destination.Index)
                    {
                        Logger.Log.Debug("Pathfinding time: " + (DateTime.Now - begin).Milliseconds);
                        //Logger.Log.Debug("Wygenerowane " + wygenerowane + " Przetworzone: " + przetworzone);
                        return current.solution;
                    }
                    else {
                        List<Field> near = current.field.GetNearestFields();
                        for (int i = 0; i < near.Count; i++)
                        {
                            if (near.ElementAt(i) != null)
                            {
                                if (near.ElementAt(i).CanPlaceUnit() && !visited.Contains(near.ElementAt(i)))
                                {
                                    State s = new State(near.ElementAt(i), current.solution, destination);
                                    frontier.Add(s);
                                    wygenerowane++;
                                }
                            }
                        }
                    }
                }
            }
            Logger.Log.Debug("Not found");
            return null;
        }
    }
}
