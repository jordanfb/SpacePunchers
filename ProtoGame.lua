--[[
This is the prototype for the game
Literally just as many joysticks as are connected get a fist to punch around with.
They may randomize tints when joining, just for ease of seeing who you are.
Punch each other!
Maybe other enemies?
]]--


ProtoFist = {}

function ProtoFist:new(args)
	local object = {}
	setmetatable(object, self)
	self.__index = self
	if args ~= nil then
		object:init(args)
	end
	return object
end

function ProtoFist:init(args)
	print("Started fist")
	self.joystick = args.joystick
	self.color = args.color or {255, 255, 255, 255}
	self.loc = {x = love.graphics.getWidth()/2, y = love.graphics.getHeight()/2, dx = 0, dy = 0, ax = 0, ay = 0, f = 0, df = 0, af = 0, goalf = 0}
	self.boostLevel = 0
	self.boostCountdown = 0 -- when this is greater than 0, you MEGA boost!
	self.boostGainMultiplier = 1.5 -- how many seconds of boost to gain per second of charging
	self.boostAngle = 0 -- set when you start charging up boost, if you get too far away, the boost fails
	self.buttonStatus = {x = 0, y = 0, a = 0, b = 0, triggerright = 0, triggerleft = 0, leftx = 0, lefty = 0, rightx = 0, righty = 0}
	--{leftFlame = false, rightFlame = false, mainFlame = false, }
	self.maxSpeed = 200
	self.drawVars = {flameTimer = 0, flameFrameTime = 0.1, flipVars = {main = 1, left = 1, right = -1}} -- offset the side flames by like .1 seconds so they don't obviously change at the same time
	self.images = {fist = love.graphics.newImage("fist.png"), mainFlame = love.graphics.newImage("flame1.png"), leftFlame = love.graphics.newImage("left flame.png"),
									rightFlame = love.graphics.newImage("right flame.png")}
end

function ProtoFist:gamepadpressed(gamepad, button)
	self.buttonStatus[button] = 1
	print(button)
	if button == "start" then
		self.loc.x = love.graphics.getWidth()/2
		self.loc.y = love.graphics.getHeight()/2
	elseif button == "back" then
		love.event.quit()
	end
end

function ProtoFist:gamepadreleased(gamepad, button)
	self.buttonStatus[button] = 0
end

function ProtoFist:gamepadaxis(gamepad, axis, value)
	self.buttonStatus[axis] = value
	-- print(axis)
	-- this below is my tested and rather failed attempt to make it turn towards the direction you want it to go
	-- I think it would probably work better with a faster turn speed and faster lockon to the right direction
	-- make it a better PID
	-- if axis == "leftx" or axis == "lefty" then
	-- 	if self.buttonStatus.lefty ~= 0 or self.buttonStatus.leftx ~= 0 then
	-- 		self.loc.goalf = math.atan2(self.buttonStatus.lefty, self.buttonStatus.leftx)
	-- 		self.loc.goalf = self.loc.goalf + 2*math.pi
	-- 		self.loc.goalf = self.loc.goalf % (2*math.pi)
	-- 		print(self.loc.goalf)
	-- 	end
	-- end
end

function ProtoFist:update(dt)
	self:physicsUpdate(dt)
	self:graphicsUpdate(dt)
end

function ProtoFist:draw(camera)
	love.graphics.setColor(255, 255, 255, 255) -- draw the flames regular color
	-- or don't, if they aren't firing...
	if self.buttonStatus.triggerright > self.buttonStatus.triggerleft then
		-- draw the main flames!
		self:drawRelative(self.images.mainFlame, -220, 15, 1*.25*camera.scale, .25*self.drawVars.flipVars.main*camera.scale, camera)
	end
	if self.buttonStatus.leftx < 0 then
		self:drawRelative(self.images.leftFlame, -100, 70, self.drawVars.flipVars.left*camera.scale*.25, camera.scale*.25, camera)
	end
	if self.buttonStatus.leftx > 0 then
		self:drawRelative(self.images.rightFlame, -100, -55, self.drawVars.flipVars.right*camera.scale*.25, camera.scale*.25, camera)
	end

	love.graphics.setColor(self.color)
	love.graphics.draw(self.images.fist, self.loc.x, self.loc.y, self.loc.f, camera.scale*.25, camera.scale*.25, self.images.fist:getWidth()/2, self.images.fist:getHeight()/2)
end

function ProtoFist:drawRelative(image, dx, dy, xscale, yscale, camera)
	-- draws the image relative to the ship location and facing to make it easier to draw flames
	local sin = math.sin(self.loc.f)
	local cos = math.cos(self.loc.f)
	local x = self.loc.x + cos*dx*camera.scale + sin*dy*camera.scale
	local y = self.loc.y + sin*dx*camera.scale - cos*dy*camera.scale
	love.graphics.draw(image, x, y, self.loc.f, xscale, yscale, image:getWidth()/2, image:getHeight()/2)
end

function ProtoFist:physicsUpdate(dt)
	-- this commented out portion is from the PID attempt. It should be tuned better.
	-- if (self.buttonStatus.leftx*self.buttonStatus.leftx)+(self.buttonStatus.leftx*self.buttonStatus.leftx) > 0 then
	-- 	if math.abs(self.loc.goalf - self.loc.f) < math.abs(self.loc.goalf - (self.loc.f-2*math.pi)) then
	-- 		self.loc.af = (self.loc.goalf - self.loc.f)
	-- 	else
	-- 		self.loc.af = (self.loc.goalf - (self.loc.f-2*math.pi))
	-- 	end
	-- else
	-- 	self.loc.af = -self.loc.df/2
	-- end
	if math.abs(self.buttonStatus.leftx) > 0 then
		self.loc.af = self.buttonStatus.leftx * 10
	else
		self.loc.af = - self.loc.df/2
	end
	if self.buttonStatus.triggerright > self.buttonStatus.triggerleft then
		-- move the fist ship
		local diff = self.buttonStatus.triggerright - self.buttonStatus.triggerleft
		local speedConstant = self.maxSpeed
		self.color = {255, 255, 255}
		self.boosting = false
		if self.boostCountdown > 0 then
			self.boostCountdown = self.boostCountdown - dt
			speedConstant = self.maxSpeed * 3
			self.color = {255, 100, 100}
			self.boosting = true
		end
		self.loc.ax = math.cos(self.loc.f) * diff * speedConstant
		self.loc.ay = math.sin(self.loc.f) * diff * speedConstant
	elseif self.buttonStatus.triggerleft > 0 then
		-- slow down fast!
		if self.boosting == true then
			self.boosting = false
			self.boostCountdown = 0
			-- print("failure from falling edge")
			self.color = {255, 255, 255}
		end
		self.boostCountdown = math.max(0, self.boostCountdown)
		self.boostCountdown = self.boostCountdown + self.buttonStatus.triggerright * dt * self.boostGainMultiplier
		if self.buttonStatus.triggerright == 0 then
			self.boostCountdown = 0
			-- print("failure from letting go")
			self.color = {255, 255, 255}
		end
		self.loc.ax = self.loc.dx * (-self.buttonStatus.triggerleft * 2)
		self.loc.ay = self.loc.dy * (-self.buttonStatus.triggerleft * 2)
	else
		self.loc.ax = -self.loc.dx / 5
		self.loc.ay = -self.loc.dy / 5
		if self.boosting == true then -- if you aren't pressing anything then you should stop getting boost!
			self.boosting = false
			self.boostCountdown = 0
			self.color = {255, 255, 255}
		end
	end
	self.loc.df = self.loc.df + self.loc.af * dt
	self.loc.dx = self.loc.dx + self.loc.ax * dt
	self.loc.dy = self.loc.dy + self.loc.ay * dt
	self.loc.x = self.loc.x + self.loc.dx * dt
	self.loc.y = self.loc.y + self.loc.dy * dt
	self.loc.f = self.loc.f + self.loc.df * dt
	self.loc.f = self.loc.f + 2*math.pi
	self.loc.f = self.loc.f % (2*math.pi)

	-- this loops the whole thing:
	local edgeSpace = 50
	if self.loc.x > love.graphics.getWidth() + edgeSpace then
		self.loc.x = -edgeSpace
	elseif self.loc.x < -edgeSpace then
		self.loc.x = love.graphics.getWidth() + edgeSpace
	end
	if self.loc.y > love.graphics.getHeight() + edgeSpace then
		self.loc.y = -edgeSpace
	elseif self.loc.y < -edgeSpace then
		self.loc.y = love.graphics.getHeight() + edgeSpace
	end
end

function ProtoFist:graphicsUpdate(dt)
	self.drawVars.flameTimer = self.drawVars.flameTimer + dt
	if self.drawVars.flameTimer > self.drawVars.flameFrameTime then
		self.drawVars.flameTimer = self.drawVars.flameTimer - self.drawVars.flameFrameTime
		-- and we flip whatever needs to be flipped.
		self.drawVars.flipVars.main = self.drawVars.flipVars.main * -1
		self.drawVars.flipVars.left = self.drawVars.flipVars.left * -1
		self.drawVars.flipVars.right = self.drawVars.flipVars.right * -1
	end
end

function ProtoFist:drawFlames()
	--
end



ProtoGame = {}

function ProtoGame:new(args)
	local object = {}
	setmetatable(object, self)
	self.__index = self
	if args ~= nil then
		object:init(args)
	end
	return object
end

function ProtoGame:init(args)
	self.camera = {scale = .5}

	self.players = {} -- when a joystick is added, then it makes a new player, indexed by numbers, probably?
	self.log = {} -- add things to this so it can draw it?

	for k, v in pairs(love.joystick.getJoysticks()) do
		self:gamepadadded(v)
	end
end

function ProtoGame:gamepadadded(joystick)
	self.players[joystick:getID()] = ProtoFist:new()
	self.players[joystick:getID()]:init{joystick = joystick}
	print("Gamepad id")
	print(joystick:getID())
end

function ProtoGame:gamepadremoved(joystick)
	self.players[joystick:getID()] = nil -- just delete it for now
end

function ProtoGame:update(dt)
	for k, v in pairs(self.players) do
		v:update(dt)
	end
end

function ProtoGame:gamepadaxis(gamepad, axis, value)
	self.players[gamepad:getID()]:gamepadaxis(gamepad, axis, value)
end

function ProtoGame:gamepadpressed(gamepad, button)
	self.players[gamepad:getID()]:gamepadpressed(gamepad, button)
end

function ProtoGame:gamepadreleased(gamepad, button)
	self.players[gamepad:getID()]:gamepadreleased(gamepad, button)
end

function ProtoGame:draw()
	for k, v in pairs(self.players) do
		v:draw(self.camera)
	end
	self:drawLog()
end

function ProtoGame:drawLog()
	love.graphics.setColor(255, 255, 255)
	y = 0
	for k, line in ipairs(self.log) do
		love.graphics.printf(line, 5, y, 1000)
		y = y + 20
	end
end

function ProtoGame:keypressed(key, unicode)
	--
end

function ProtoGame:keyreleased(key, unicode)
	--
end

function ProtoGame:mousepressed(x, y, button)
	--
end

function ProtoGame:mousereleased(x, y, button)
	--
end

function ProtoGame:resize(width, height)
	--
end