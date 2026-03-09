import RPi.GPIO as GPIO
class TB6612FNG:
    def __init__(self, pwm_pin, a1_pin, a2_pin, b1_pin, b2_pin):
        self.pwm_pin = pwm_pin
        self.a1_pin = a1_pin
        self.a2_pin = a2_pin
        self.b1_pin = b1_pin
        self.b2_pin = b2_pin
        
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(self.a1_pin, GPIO.OUT)
        GPIO.setup(self.a2_pin, GPIO.OUT)
        GPIO.setup(self.b1_pin, GPIO.OUT)
        GPIO.setup(self.b2_pin, GPIO.OUT)
        GPIO.setup(self.pwm_pin, GPIO.OUT)
        
        self.pwm = GPIO.PWM(self.pwm_pin, 1000)
        self.pwm.start(0)

    def forward(self, speed):
        GPIO.output(self.a1_pin, GPIO.HIGH)
        GPIO.output(self.a2_pin, GPIO.LOW)
        self.set_speed(speed)

    def backward(self, speed):
        GPIO.output(self.b1_pin, GPIO.HIGH)
        GPIO.output(self.b2_pin, GPIO.LOW)
        self.set_speed(speed)

    def stop(self):
        GPIO.output(self.a1_pin, GPIO.LOW)
        GPIO.output(self.a2_pin, GPIO.LOW)
        GPIO.output(self.b1_pin, GPIO.LOW)
        GPIO.output(self.b2_pin, GPIO.LOW)
        self.set_speed(0)

    def set_speed(self, speed):
        if 0 <= speed <= 100:
            self.pwm.ChangeDutyCycle(speed)
        else:
            raise ValueError("Speed must be between 0 and 100")

    def cleanup(self):
        self.pwm.stop()
        GPIO.cleanup()