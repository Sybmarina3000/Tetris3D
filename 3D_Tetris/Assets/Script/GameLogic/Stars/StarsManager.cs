using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script.GameLogic.Stars
{
    class StarInfo
    {
        public Block block;
        public float angle;

        public StarInfo(Block block, float angle)
        {
            this.block = block;
            this.angle = angle;
        }
    }

    public class StarsManager : MonoBehaviour
    {
        public bool collectStarLvlLvl { get { return _collectStarsLvl; } set 
        { _collectStarsLvl = value; 
            if (!_collectStarsLvl) {RealizationBox.Instance.starUI.gameObject.SetActive(false);}
        } }
    
        public int collectedStars { get; private set; }
        public int neededStars { get { return _neededStars;} set { _neededStars = value; } }
        public int maxStarsAmount { get { return _maxStarsAmount;} set { _maxStarsAmount = value; } }
        
        [SerializeField] private bool _collectStarsLvl;
        [SerializeField] private int _maxStarsAmount = 1;
        [SerializeField] private int _neededStars = 5;
        
        public Action OnCreatedStar;
        public Action OnUpdatedCollectingStars;
        public Action OnCollectedStars;
        public bool onCollectedAnimationWaiting { get; private set; }

        [SerializeField] private Mesh _starMesh;
        [SerializeField] private Material _starmaterial;

        // Animation
        [Header("Animation")]
        [SerializeField] private Transform _rotationStar;
        [SerializeField] private Transform _animationStar;
        [SerializeField] private float _highestPathPos = 32;

        [SerializeField] private float _firstPointR = 2.5f;
        [SerializeField] private float _secondPointR = 2.5f;
        [SerializeField] private float _secondPointYP = 2.5f;
        private List<Vector3> _animationPath;
        
        [SerializeField] private float _starRotationSpeed = 20.0f;
        [SerializeField] private float _fallStarRotationSpeed = 300.0f;
        private List<StarInfo> _rotationStars;
        private float _fallStarAngle = 0;
        
        [SerializeField] private ParticleSystem _particles;
        private bool isParticle = false;
        // End of animation
        
        private List<Block> _stars;
        private List<Block> _applicants;
        
        private PlaneMatrix _matrix;
        private Transform _cameraTransform;

        [SerializeField] private Transform _lookAtStar;
      
        private void Start()
        {
            _rotationStars = new List<StarInfo>();
            _stars = new List<Block>();
            _applicants = new List<Block>();
            _animationPath = new List<Vector3>() {new Vector3(0, _highestPathPos, 0), Vector3.zero};
            
            _matrix = RealizationBox.Instance.matrix;
            _cameraTransform = RealizationBox.Instance.gameCamera.transform;
        }

        private void Update()
        {
            foreach (var s in _rotationStars)
            {
                s.block.Star.LookAt(Camera.main.transform);
                s.angle += Time.deltaTime *_starRotationSpeed;
                if (s.angle > 360.0f)
                {
                    s.angle = 0.0f;
                }
                s.block.oreol.localRotation = Quaternion.Euler(0, 0, s.angle);
            }
            
            
        }

        public bool CanCreateStar()
        {
            if (_stars.Count >= _maxStarsAmount)
                return false;
            
            _applicants.Clear();
            for (int i = 0; i <= _matrix.height; i++)
            {
                if (_stars.Exists((Block b) => { return b._coordinates.y == i; }))
                    continue;

                AddApplicants(0, i, 0);
                AddApplicants(0, i, _matrix.wight-1);
                AddApplicants(_matrix.wight-1, i, 0);
                AddApplicants(_matrix.wight-1, i, _matrix.wight-1);
            }
            
            if (_applicants.Count > 0)
                return true;
            return false;
        }
        
        void AddApplicants(int x, int y, int z)
        {
            var b = _matrix.GetBlockInPlace(x, y, z);
            if (b)
            {   
                _applicants.Add(b);    
            }
        }

        public void CreateStar()
        {
            var rndBlock = _applicants[Random.Range(0, _applicants.Count)];
            
            _stars.Add(rndBlock);
            rndBlock.OnCollected += CollectStar;
            CreateAnimation(rndBlock);
        }

        public void CreateAnimation(Block block)
        {
            isParticle = false;
            _animationStar.transform.localPosition = Vector3.zero;
            _animationStar.transform.localRotation = Quaternion.identity;
            
            Vector3 endPosition =  _animationStar.InverseTransformPoint(block.myTransform.position);;
            
            _animationPath.Add(endPosition);
            _animationPath[0] = new Vector3(endPosition.x * _firstPointR, _animationPath[0].y, endPosition.z * _firstPointR);
            _animationPath[1] = new Vector3(endPosition.x * _secondPointR + 0.5f * _secondPointR  , endPosition.y + _secondPointYP, endPosition.z * _secondPointR + 0.5f * _secondPointR );

            _animationStar.gameObject.SetActive(true);
            _animationStar.localPosition = _animationPath[0];
            _animationStar.LookAt(_cameraTransform);//Camera.main.transform);
            _animationStar.DOLocalPath(_animationPath.ToArray(), 2, PathType.CatmullRom, PathMode.Ignore).
                    OnUpdate(() =>
                    {
                        _fallStarAngle += Time.deltaTime *_fallStarRotationSpeed;
                        if (_fallStarAngle > 360.0f)
                            {
                                _fallStarAngle = 0.0f;
                            }
                        _rotationStar.localRotation = Quaternion.Euler(0, 0, _fallStarAngle);
                        _lookAtStar.LookAt(_cameraTransform.position);

                        if (!isParticle && Vector3.Distance(_animationStar.transform.localPosition, _animationPath[2]) < 1)
                        {
                            _particles.transform.position = block.myTransform.position;
                            _particles.gameObject.SetActive(false);
                            _particles.gameObject.SetActive(true);

                            isParticle = true;
                        }
                    }).
              OnComplete(()=> 
              {
                  block.TransformToStar(_starMesh, _starmaterial);
                  
                  _animationPath.RemoveAt(2);
                  _animationStar.DOKill();
                  _animationStar.gameObject.SetActive(false);
                  
                  OnCreatedStar?.Invoke();
                  _rotationStars.Add(new StarInfo(block, 0));
              });

            _animationStar.DOLocalRotate(Vector3.forward, 4).SetLoops(-1,LoopType.Incremental);
        }

        public void CollectStar(Block star)
        {
            star.OnCollected -= CollectStar;
            collectedStars++;
            _stars.Remove(star);

            StarInfo rStr;
            foreach (var s in _rotationStars)
            {
                if (s.block == star)
                {
                    rStr = s;
                    _rotationStars.Remove(rStr);
                    break;
                }
            }

            //  OnUpdatedCollectingStars?.Invoke();
            if (!onCollectedAnimationWaiting)
            {
                onCollectedAnimationWaiting = true;
                RealizationBox.Instance.starUIAnimation.OnAnimationEnd += FinishCollectAnimation;
             }
            RealizationBox.Instance.starUIAnimation.StartAnimation();
        }

        public void FinishCollectAnimation() // doesnt work when few stars are collected
        {
            onCollectedAnimationWaiting = false;
            RealizationBox.Instance.starUIAnimation.OnAnimationEnd -= FinishCollectAnimation;
            
            OnCollectedStars?.Invoke();
        }

        public void Clear()
        {
            _stars.Clear();
            _applicants.Clear();
            collectedStars = 0;
            OnUpdatedCollectingStars?.Invoke();
        }

        public bool CheckWin()
        {
            if (!collectStarLvlLvl)
                return true;
            return collectedStars >= neededStars;
        }
    }
}