class Calculator {
    constructor() {
        this.currentDisplay = "0";
        this.currentEquation = "";
        this.lastOperation = "";
        this.storedValue = 0;
        this.isNewCalculation = true;
        this.initializeEvents();
    }

    initializeEvents() {
        document.querySelectorAll('.key').forEach(button => {
            button.addEventListener('click', (e) => {
                this.handleButtonClick(e.target.textContent);
            });
        });
    }

    async handleButtonClick(buttonText) {
        let action = this.getActionType(buttonText);
        let value = this.getButtonValue(buttonText);

        const request = {
            action: action,
            value: value,
            currentDisplay: this.currentDisplay,
            currentEquation: this.currentEquation,
            lastOperation: this.lastOperation,
            storedValue: this.storedValue,
            isNewCalculation: this.isNewCalculation
        };

        try {
            const response = await fetch('/?handler=Calculator', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify(request)
            });

            if (response.ok) {
                const result = await response.json();
                this.updateDisplay(result);
            }
        } catch (error) {
            console.error('Calculator error:', error);
        }

        //console.log('Button clicked:', buttonText);
        //console.log('Request:', request);
    }

    getActionType(buttonText) {
        const numberButtons = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
        const operatorButtons = ['+', '-', '×', '÷'];

        if (numberButtons.includes(buttonText)) return 'number';
        if (operatorButtons.includes(buttonText)) return 'operator';
        if (buttonText === '=') return 'equals';
        if (buttonText === 'C') return 'clear';
        if (buttonText === '+/-') return 'plusminus';
        if (buttonText === '%') return 'percent';
        if (buttonText === '.') return 'decimal';
        if (buttonText === 'dx') return 'delete';

        return 'unknown';
    }

    getButtonValue(buttonText) {
        const operatorMap = {
            '+': 'add',
            '-': 'subtract',
            '×': 'multiply',
            '÷': 'divide'
        };

        return operatorMap[buttonText] || buttonText;
    }

    updateDisplay(result) {
        this.currentDisplay = result.currentDisplay;
        this.currentEquation = result.currentEquation;
        this.lastOperation = result.lastOperation;
        this.storedValue = result.storedValue;
        this.isNewCalculation = result.isNewCalculation;

        document.querySelector('.result').textContent = result.currentDisplay;
        document.querySelector('.equation').textContent = result.currentEquation;

        if (result.hasError) {
            document.querySelector('.result').textContent = result.errorMessage;
        }
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new Calculator();
});