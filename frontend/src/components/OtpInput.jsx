import { useEffect, useRef } from 'react';

export default function OtpInput({ value, onChange, disabled = false }) {
  const inputsRef = useRef([]);

  useEffect(() => {
    inputsRef.current[0]?.focus();
  }, []);

  const digits = value.padEnd(4, ' ').slice(0, 4).split('');

  function handleChange(index, event) {
    const digit = event.target.value.replace(/\D/g, '').slice(-1);
    const next = digits.map((item, i) => (i === index ? digit : item.trim())).join('');
    onChange(next.replace(/\s/g, ''));

    if (digit && index < 3) {
      inputsRef.current[index + 1]?.focus();
    }
  }

  function handleKeyDown(index, event) {
    if (event.key === 'Backspace' && !digits[index]?.trim() && index > 0) {
      inputsRef.current[index - 1]?.focus();
    }
  }

  function handlePaste(event) {
    event.preventDefault();
    const pasted = event.clipboardData.getData('text').replace(/\D/g, '').slice(0, 4);
    onChange(pasted);
    const focusIndex = Math.min(pasted.length, 3);
    inputsRef.current[focusIndex]?.focus();
  }

  return (
    <div className="tijori-otp-group">
      {digits.map((digit, index) => (
        <input
          key={index}
          ref={(element) => {
            inputsRef.current[index] = element;
          }}
          className="tijori-otp-input"
          inputMode="numeric"
          maxLength={1}
          value={digit.trim()}
          disabled={disabled}
          onChange={(event) => handleChange(index, event)}
          onKeyDown={(event) => handleKeyDown(index, event)}
          onPaste={handlePaste}
          aria-label={`OTP digit ${index + 1}`}
        />
      ))}
    </div>
  );
}
