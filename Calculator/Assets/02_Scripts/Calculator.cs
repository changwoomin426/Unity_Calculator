using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calculator : MonoBehaviour {

    private string _expr;


    [Header("--- UI")]
    [SerializeField] private TextMeshProUGUI _textMesh;
    [SerializeField] private Button[] _numberButtons;
    [SerializeField] private Button _plusButton;
    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _multipleButton;
    [SerializeField] private Button _divideButton;
    [SerializeField] private Button _modButton;
    [SerializeField] private Button _equalButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _backspaceButton;

    private void Awake() {
        Initialize();
    }

    private void Start() {
        Clear();
    }

    private void Initialize() {

        for (int i = 0; i < _numberButtons.Length; i++) {
            int digit = i;
            _numberButtons[i].onClick.AddListener(() => OnClickNumberButton(digit));
        }

        _plusButton.onClick.AddListener(() => OnClickOperator('+'));
        _minusButton.onClick.AddListener(() => OnClickOperator('-'));
        _multipleButton.onClick.AddListener(() => OnClickOperator('*'));
        _divideButton.onClick.AddListener(() => OnClickOperator('/'));
        _modButton.onClick.AddListener(() => OnClickOperator('%'));


        _equalButton.onClick.AddListener(OnClickEqual);
        _clearButton.onClick.AddListener(Clear);
        _backspaceButton.onClick.AddListener(Backspace);
    }

    private void Clear() {
        _expr = "";
        _textMesh.text = "0";
    }

    private void RefreshText() {
        _textMesh.text = string.IsNullOrEmpty(_expr) ?
            "0" : _expr;
    }

    private void OnClickNumberButton(int digit) {

        if (string.IsNullOrEmpty(_expr)) {

            if (digit == 0) {
                _expr = "0";
                RefreshText();
                return;
            }

            _expr += digit.ToString();
            RefreshText();
            return;
        }

        int lastOpIndex = LastOperatorIndex(_expr);

        int numberStart = lastOpIndex + 1;
        string currentNumber = _expr.Substring(numberStart);

        if (currentNumber == "0") {

            if (digit == 0) {
                RefreshText();
                return;
            }

            _expr = _expr.Substring(0, numberStart) + digit;
            RefreshText();
            return;
        }

        _expr += digit.ToString();
        RefreshText();
    }

    private int LastOperatorIndex(string s) {

        for (int i = s.Length - 1; i >= 0; i--) {

            if (IsOperator(s[i])) {
                return i;
            }

        }

        return -1;
    }

    private void OnClickOperator(char op) {

        if (string.IsNullOrEmpty(_expr)) {

            if (op == '-') {
                _expr = "-";
                RefreshText();
            }

            return;
        }

        char last = _expr[_expr.Length - 1];

        if (IsOperator(last)) {

            if (op == '-' && last != '-') {
                _expr += '-';
                RefreshText();
                return;
            }

            _expr = _expr.Substring(0, _expr.Length - 1) + op;
            RefreshText();
            return;
        }

        _expr += op;
        RefreshText();
    }

    private void Backspace() {
        if (string.IsNullOrEmpty(_expr)) {
            return;
        }

        _expr = _expr.Substring(0, _expr.Length - 1);
        RefreshText();
    }

    private void OnClickEqual() {
        if (string.IsNullOrEmpty(_expr)) {
            return;
        }

        while (_expr.Length > 0 &&
                IsOperator(_expr[_expr.Length - 1])) {
            _expr = _expr.Substring(0, _expr.Length - 1);
        }

        if (string.IsNullOrEmpty(_expr)) {
            Clear();
            return;
        }

        try {
            int result = Evaluate(_expr);
            _expr = result.ToString();
        } catch (System.DivideByZeroException) {
            _expr = "";
            _textMesh.text = "Error";
            return;
        } catch {
            _expr = "";
            _textMesh.text = "Error";
            return;
        }

        RefreshText();
    }

    private bool IsOperator(char c) {
        return c == '+' ||
                c == '-' ||
                c == '*' ||
                c == '/' ||
                c == '%';
    }



    private int Evaluate(string expr) {
        List<string> tokens = Tokenize(expr);

        for (int i = 0; i < tokens.Count; i++) {
            if (tokens[i] == "*" ||
                tokens[i] == "/" ||
                tokens[i] == "%") {

                int left = int.Parse(tokens[i - 1]);
                int right = int.Parse(tokens[i + 1]);

                if ((tokens[i] == "/" || tokens[i] == "%") && right == 0) {
                    throw new System.DivideByZeroException();
                }

                int value;

                if (tokens[i] == "*") {
                    value = left * right;
                } else if (tokens[i] == "/") {
                    value = left / right;
                } else {
                    value = left % right;
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

        for (int i = 0; i < expr.Length; i++) {
            char c = expr[i];

            if (c == '-' && (i == 0 || IsOperator(expr[i - 1]))) {
                number += c;
                continue;
            }

            if (char.IsDigit(c)) {
                number += c;
            } else {

                if (number.Length > 0) {
                    tokens.Add(number);
                }

                tokens.Add(c.ToString());
                number = "";
            }
        }

        if (number.Length > 0) {
            tokens.Add(number);
        }

        return tokens;
    }


}
