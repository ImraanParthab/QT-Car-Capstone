import Jetson.GPIO as GPIO

def setup_pin(pin, mode):
    
    GPIO.setmode(GPIO.BOARD)
    GPIO.setup(pin, mode)

def set_pin_high(pin):
    
    GPIO.output(pin, GPIO.HIGH)

def set_pin_low(pin):
    
    GPIO.output(pin, GPIO.LOW)

def cleanup():
    
    GPIO.cleanup()