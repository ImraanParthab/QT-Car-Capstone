import Jetson.GPIO as GPIO
from time import sleep

a1 = 29
a2 = 31
apwm = 33

pins = [a1, a2, apwm]
GPIO.setmode(GPIO.BOARD)

for pin in pins:
    GPIO.setup(pin, GPIO.OUT, initial=GPIO.HIGH)

# Initialize PWM once
pwm = GPIO.PWM(apwm, 100)
pwm.start(50)  # Start PWM with 50% duty cycle

try:
    while True:
        # Forward
        GPIO.output(a1, GPIO.HIGH)
        GPIO.output(a2, GPIO.LOW)
        pwm.ChangeDutyCycle(100)
        sleep(2)

        # Reverse
        GPIO.output(a1, GPIO.LOW)
        GPIO.output(a2, GPIO.HIGH)
        pwm.ChangeDutyCycle(100)
        sleep(2)

except KeyboardInterrupt:
    pass

finally:
    pwm.stop()
    GPIO.cleanup()