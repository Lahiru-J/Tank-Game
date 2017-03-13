using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

namespace Assets.Model
{
    public static class AI
    {
        public static Direction getMovemnt(int playerX, int playerY, int coinX, int coinY)
        {
            var matrix = new int[10, 10];
            matrix[playerX, playerY] = 0;
            fillMatrix(matrix, playerX, playerY);
            return new Direction();
        }

        private static void fillMatrix(int[,] matrix, int currentX, int currentY)
        {
            fillMatrix(matrix, currentX - 1, currentY - 1);
            fillMatrix(matrix, currentX + 1, currentY + 1);
            fillMatrix(matrix, currentX + 1, currentY - 1);
            fillMatrix(matrix, currentX - 1, currentY + 1);
        }

        private static int getFillingValue(int posX, int posY)
        {
            
            return 0;
        }

        public static void moveTank()
        {
            
        }
    }
}
