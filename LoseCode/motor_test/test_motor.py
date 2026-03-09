import unittest

class Motor:
    def __init__(self, tb6612_driver):
        self.tb6612 = tb6612_driver

    def forward(self, speed=1.0):
        self.tb6612.set_speed(speed)
        self.tb6612.set_direction('forward')

    def backward(self, speed=1.0):
        self.tb6612.set_speed(speed)
        self.tb6612.set_direction('backward')

    def stop(self):
        self.tb6612.set_speed(0)

    def set_speed(self, speed):
        self.tb6612.set_speed(speed)

class TestMotor(unittest.TestCase):
    def setUp(self):
        self.motor = Motor()

    def test_forward(self):
        self.motor.forward()
        self.assertTrue(self.motor.is_moving_forward())

    def test_backward(self):
        self.motor.backward()
        self.assertTrue(self.motor.is_moving_backward())

    def test_stop(self):
        self.motor.stop()
        self.assertFalse(self.motor.is_moving())

    def test_set_speed(self):
        self.motor.set_speed(100)
        self.assertEqual(self.motor.get_speed(), 100)

if __name__ == '__main__':
    unittest.main()