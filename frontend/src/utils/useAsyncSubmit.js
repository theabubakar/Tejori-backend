import { useCallback, useRef, useState } from 'react';

export function useAsyncSubmit() {
  const lockRef = useRef(false);
  const [submitting, setSubmitting] = useState(false);

  const runSubmit = useCallback(async (task) => {
    if (lockRef.current) {
      return null;
    }

    lockRef.current = true;
    setSubmitting(true);

    try {
      return await task();
    } finally {
      lockRef.current = false;
      setSubmitting(false);
    }
  }, []);

  return { submitting, runSubmit };
}
