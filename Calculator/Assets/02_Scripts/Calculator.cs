using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour {

    [SerializeField] private string _expr;



    private void Start() {
        int result = Evaluate(_expr);
        Debug.Log(result);
    }


    private int Evaluate(string expr) {
        List<string> tokens = Tokenize(expr);

        for (int i = 0; i < tokens.Count; i++) {
            if (tokens[i] == "*" ||
                tokens[i] == "/") {
                int left = int.Parse(tokens[i - 1]);
                int right = int.Parse(tokens[i + 1]);

                int value;

                if (tokens[i] == "*") {
                    value = left * right;
                } else {
                    value = left / right;
                }

                tokens[i - 1] = value.ToString();
                tokens.RemoveAt(i);
                tokens.RemoveAt(i);
                i--;
            }
        }

        int result = int.Parse(tokens[0]);

        for (int i = 1; i < tokens.Count; i += 2) {
            int value = int.Parse(tokens[i + 1]);

            if (tokens[i] == "+") {
                result = result + value;
            } else {
                result = result - value;
            }
        }

        return result;
    }


    private List<string> Tokenize(string expr) {
        List<string> tokens = new List<string>();
        string number = "";

        foreach (char c in expr) {
            if (char.IsDigit(c)) {
                number += c;
            } else {
                tokens.Add(number);
                tokens.Add(c.ToString());
                number = "";
            }
        }

        tokens.Add(number);
        return tokens;
    }

}
