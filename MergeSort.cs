using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MultiThreadSort
{
    public class MergeSort
    {
        #region Helper Functions [TASK 1]
        public static object Params2Object(int[] A, int s, int e, int m, int node_idx)
        {
            ArrayList parametersList = new ArrayList();
            parametersList.Add(A);
            parametersList.Add(s);
            parametersList.Add(e);
            parametersList.Add(m);
            parametersList.Add(node_idx);
            return parametersList;
        }


        public static void Object2Params(object parameters, ref int[] A, ref int s, ref int e, ref int m, ref int node_idx)
        {
            ArrayList parametersList = (ArrayList)parameters;
            A = (int[])parametersList[0];
            s = (int)parametersList[1];
            e = (int)parametersList[2];
            m = (int)parametersList[3];
            node_idx = (int)parametersList[4];
        }

        #endregion

        //DO NOT CHANGE THIS CODE
        #region Sequential Sort 

        public static void Sort(int[] array)
        {
            MSort(array, 1, array.Length);
        }

        private static void MSort(int[] A, int s, int e)
        {
            if (s >= e)
            {
                return;
            }

            int m = (s + e) / 2;

            MSort(A, s, m);

            MSort(A, m + 1, e);

            Merge(A, s, m, e);
        }

        private static void Merge(int[] A, int s, int m, int e)
        {
            int leftCapacity = m - s + 1;

            int rightCapacity = e - m;

            int leftIndex = 0;

            int rightIndex = 0;

            int[] Left = new int[leftCapacity];

            int[] Right = new int[rightCapacity];

            for (int i = 0; i < Left.Length; i++)
            {
                Left[i] = A[s + i - 1];
            }

            for (int j = 0; j < Right.Length; j++)
            {
                Right[j] = A[m + j];
            }

            for (int k = s; k <= e; k++)
            {
                if (leftIndex < leftCapacity && rightIndex < rightCapacity)
                {
                    if (Left[leftIndex] < Right[rightIndex])
                    {
                        A[k - 1] = Left[leftIndex++];
                    }
                    else
                    {
                        A[k - 1] = Right[rightIndex++];
                    }
                }
                else if (leftIndex < leftCapacity)
                {
                    A[k - 1] = Left[leftIndex++];
                }
                else
                {
                    A[k - 1] = Right[rightIndex++];
                }
            }
        }
        #endregion
        //TODO: Change this function to be MULTITHREADED
        //HINT: Remember to handle any dependency and/or critical section issues
        #region Multithreaded Sort [REMAINING TASKS]
        static int NumMergeSortThreads;
        #region Semaphores
        //TODO: Define any required semaphore here
        static Semaphore semMergeSort = new Semaphore();
        static Semaphore semMerge = new Semaphore();
        #endregion
        #region Threads
        //TODO: Define any required thread objects here
        #endregion
        #region Sort Function
        public static void SortMT(int[] array)
        {
            int s = 1;
            int e = array.Length;
            int m = (s + e) / 2;
            int node_idx = 0;
            int n = e - s + 1;
            //TASK2
            int sm = (s + m) / 2; 
            int me = ((m + e + 1) / 2);
            //TASK3
            int r = ((s + e) / 4);
            int sr = (s + ((s + e) / 4)) / 2;
            int rm = ((((s + e) / 4) + 1) + ((s + e) / 2)) / 2;
            int t = (3 * (s + e)) / 4;
            int mt = ((((s + e) / 2) + 1) + ((3 * (s + e)) / 4)) / 2;
            int te = ((((3 * (s + e)) / 4) + 1) + e) / 2;
            NumMergeSortThreads = 2;                //TASK 2
            //NumMergeSortThreads = 4;              //TASK 3
            //NumMergeSortThreads = 8;
            if (NumMergeSortThreads == 2)   //TASK 2
            {
                Thread Thread1 = new Thread(MSortMT);
                Thread1.Start(Params2Object(array, s, m, sm, node_idx));
                Thread Thread2 = new Thread(MSortMT);
                Thread2.Start(Params2Object(array, m + 1, e, me, node_idx));
                for (int i = 0; i < 2; i++)
                {
                    semMergeSort.Wait();
                }
                Thread merge = new Thread(MergeMT);
                merge.Start(Params2Object(array, s, e, m, node_idx));
                semMerge.Wait();
            }
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                
                Thread Thread1 = new Thread(MSortMT);
                Thread1.Start(Params2Object(array, s, r, sr, node_idx));

                Thread Thread2 = new Thread(MSortMT);
                Thread2.Start(Params2Object(array, r + 1, m, rm, node_idx));

                Thread Thread3 = new Thread(MSortMT);
                Thread3.Start(Params2Object(array, m + 1, t, mt, node_idx));

                Thread Thread4 = new Thread(MSortMT);
                Thread4.Start(Params2Object(array, t + 1, e, te, node_idx));

                for (int i = 0; i < 4; i++) 
                {
                    semMergeSort.Wait();
                }

                Thread merge1 = new Thread(MergeMT);
                merge1.Start(Params2Object(array, s, m, sm, node_idx));
                merge1 = new Thread(MergeMT);
                merge1.Start(Params2Object(array, m + 1, e, me, node_idx));
                for(int i =0; i < 2; i++) 
                {
                    semMerge.Wait();
                }
                Thread merge2 = new Thread(MergeMT);
                merge2.Start(Params2Object(array, s, e, m, node_idx));
                semMerge.Wait();


            }
            else
            {
                int loop = 0;
                
                if (array.Length < NumMergeSortThreads)
                {
                    loop = array.Length;
                }
                else
                {
                    loop = NumMergeSortThreads;
                }

                int range = (array.Length) / loop;

               
                for (int i = 0; i < loop; ++i)
                {
                    int start = (i * range) + 1;
                    int end = (i + 1) * range;
                    int med = (start + end) / 2;

                    Thread Thread1 = new Thread(MSortMT);
                    Thread1.Start(Params2Object(array, start, end, med, node_idx));
                }

                for (int i = 0; i < loop; ++i)
                {
                    semMergeSort.Wait();
                }

                for (int i = 0; i < loop; i += range)
                {

                    int start = (i * range) + 1;//1  5
                    int end = ((i + 1) * (range)) + 2;//4 8
                    int med = (start + end) / 2;

                    Thread Thread2 = new Thread(MergeMT);
                    Thread2.Start(Params2Object(array, start, end, med, node_idx));
                    semMerge.Wait();

                }
                
                for (int i = 0; i < loop; i += range * 2)
                {

                    int start = (i * range) + 1;//1 9  
                    int end = ((i + 1) * (range)) + 6;//8 16
                    int med = (start + end) / 2;
                    Thread Thread3 = new Thread(MergeMT);
                    Thread3.Start(Params2Object(array, start, end, med, node_idx));
                    semMerge.Wait();
                }

                Thread Thread4 = new Thread(MergeMT);
                Thread4.Start(Params2Object(array, s, e, m, node_idx));
                semMerge.Wait();

            }
            #endregion

        }

        private static void MSortMT(object parameters)
        {
            #region Extract params from the given object 
            int[] A = null;
            int s = 0;
            int e = 0;
            int m = 0;
            int node_idx = 0;
            Object2Params(parameters, ref A, ref s, ref e, ref m, ref node_idx);
            #endregion


            MSort(A, s, e);

            #region [TASK 2] 
            if (NumMergeSortThreads == 2)       //TASK 2
            {
                //TODO: Use semaphores to handle any dependency or critical section
                semMergeSort.Signal();

            }
            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {
                //TODO: Use semaphores to handle any dependency or critical section
                semMergeSort.Signal();

            }
            #endregion

            #region [TASK 4]
            else
            {
                // TODO: Use semaphores to handle any dependency or critical section

                semMergeSort.Signal();
            }
            #endregion


        }

        private static void MergeMT(object parameters)
        {
            #region Extract params from the given object
            int[] A = null;
            int s = 0;
            int e = 0;
            int m = 0;
            int node_idx = 0;
            Object2Params(parameters, ref A, ref s, ref e, ref m, ref node_idx);
            #endregion

            #region [TASK 2]
            if (NumMergeSortThreads == 2)   //TASK 2
            {
                Merge(A, s, m, e);
                semMerge.Signal();
            }

            #endregion

            #region [TASK 3]
            else if (NumMergeSortThreads == 4)   //TASK 3
            {           
                Merge(A, s, m, e);
                semMerge.Signal();
            }


            #endregion

            #region [TASK 4]
            else
            {       
                Merge(A, s, m, e);
                semMerge.Signal();

            }

            #endregion
        }
        #endregion


    }
}